using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetStash.Core.Log;

namespace NetStash.Core.Worker
{
    public static class TcpWorker
    {
        static string _server = string.Empty;
        static int _port = -1;

        static readonly object Lock = new object();
        static bool _isRunning;
        static Task _run;

        static bool _stopCalled;

        /// <summary>
        /// Inicializa a sincronizaçao das mensagens com o servidor
        /// </summary>
        /// <param name="server">Nome ou IP do servidor</param>
        /// <param name="port">Porta de conexao com o servidor</param>
        /// <exception cref="ArgumentNullException">Quando o parametro server nao inforamdo</exception>
        /// <exception cref="ArgumentOutOfRangeException">Quando o parametro port for menor ou igual a zero</exception>
        public static void Initialize(string server, int port)
        {
            if (string.IsNullOrEmpty(server) || string.IsNullOrWhiteSpace(server))
                throw new ArgumentNullException(nameof(server));

            if (port <= 0)
                throw new ArgumentOutOfRangeException(nameof(port));
            
            _server = server;
            _port = port;

            Run();
        }

        /// <summary>
        /// Inicia a sincronizaçao com o servidor
        /// </summary>
        public static void Run()
        {
            if (_stopCalled) return;

            if (_run == null || _run.Status != TaskStatus.Running)
            {
                _run = Task.Factory.StartNew(() =>
                {
                    lock (Lock)
                        _isRunning = true;

                    while (_isRunning)
                    {
                        try
                        {
                            Runner();
                        }
                        catch (Exception ex)
                        {
                            NetStashLog log = new NetStashLog(_server, _port, "NetStash", "NetStash");
                            log.InternalError("Logstash communication error: " + ex.Message);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Reinicia a sincronizaçao das mensagens com o servidor
        /// </summary>
        internal static void Restart()
        {
            lock (Lock)
                _isRunning = true;

            _stopCalled = false;

            Run();
        }

        /// <summary>
        /// Para a sincronizaçao dos dados com o servidor
        /// </summary>
        internal static void Stop()
        {
            lock (Lock)
                _isRunning = false;

            _stopCalled = true;

            _run?.Wait();
        }

        /// <summary>
        /// Envia as mensagens armazenadas ao servidor
        /// </summary>
        private static void Runner()
        {
            Storage.Proxy.LogProxy proxy = new Storage.Proxy.LogProxy();
            Dictionary<long, string> evs;

            lock (Lock)
            {
                evs = proxy.GetList();
                if (evs.Count <= 0)
                {
                    _isRunning = false;
                    return;
                }
            }

            Send(evs, DeleteEntry);
        }

        /// <summary>
        /// Remove o registro de evento do banco de dados
        /// </summary>
        /// <param name="id">Codigo do evento</param>
        private static void DeleteEntry(long id)
        {
            Storage.Proxy.LogProxy proxy = new Storage.Proxy.LogProxy();
            proxy.Delete(id);
        }

        /// <summary>
        /// Envia o evento para o servidor
        /// </summary>
        /// <param name="evs">Dados do evento a serem enviados</param>
        /// <param name="after">Açao que deve ser executada apos o envio da mensagem</param>
        private static void Send(Dictionary<long, string> evs, Action<long> after)
        {
            using (TcpClient client = new TcpClient(_server, _port))
            using (StreamWriter writer = new StreamWriter(client.GetStream()))
            {
                foreach (KeyValuePair<long, string> ev in evs)
                {
                    writer.WriteLine(ev.Value.Replace(Environment.NewLine, "@($NL$)@"));
                    after(ev.Key);
                }
            }
        }
    }
}
