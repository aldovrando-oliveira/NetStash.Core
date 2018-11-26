using System;
using System.Collections.Generic;
using NetStash.Core.Worker;

namespace NetStash.Core.Log
{
    public class NetStashLog
    {
        private readonly string _logger;
        private readonly string _system;

        /// <summary>
        /// Retorna uma nova instancia de <see cref="NetStashLog"/>
        /// </summary>
        /// <param name="host">Nome do servidor</param>
        /// <param name="port">Porta do servidor</param>
        /// <param name="system">Sistema que esta sendo logado</param>
        /// <param name="logger">Componente do sistema que esta usando o Log</param>
        /// <exception cref="ArgumentNullException"></exception>
        public NetStashLog(string host, int port, string system, string logger = "NetStashLogs")
        {
            if (string.IsNullOrEmpty(host) || string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException(nameof(host));

            if (string.IsNullOrEmpty(logger) || string.IsNullOrWhiteSpace(logger))
                throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrEmpty(system) || string.IsNullOrWhiteSpace(system))
                throw new ArgumentNullException(nameof(system));

            TcpWorker.Initialize(host, port);

            _logger = logger;
            _system = system;
        }

        /// <summary>
        /// Para o serviço de sincronizaçao
        /// </summary>
        public void Stop()
        {
            TcpWorker.Stop();
        }

        /// <summary>
        /// Reinicia o serviço de sincronizaçao
        /// </summary>
        public void Restart()
        {
            TcpWorker.Restart();
        }

        /// <summary>
        /// Registra um novo log com nivel <see cref="NetStashLogLevel.Verbose"/>
        /// </summary>
        /// <param name="message">Mensagem descritiva do log</param>
        /// <param name="values">Valores adicionais que serao incluidos no log</param>
        public void Verbose(string message, Dictionary<string, string> values = null)
        {
            Log(NetStashLogLevel.Verbose, message, null, values);
        }

        /// <summary>
        /// Registra um novo log com nivel <see cref="NetStashLogLevel.Debug"/>
        /// </summary>
        /// <param name="message">Mensagem descritiva do log</param>
        /// <param name="values">Valores adicionais que serao incluidos no log</param>
        public void Debug(string message, Dictionary<string, string> values = null)
        {
            Log(NetStashLogLevel.Debug, message, null, values);
        }

        /// <summary>
        /// Registra um novo log com nivel <see cref="NetStashLogLevel.Information"/>
        /// </summary>
        /// <param name="message">Mensagem descritiva do log</param>
        /// <param name="values">Valores adicionais que serao incluidos no log</param>
        public void Information(string message, Dictionary<string, string> values = null)
        {
            Log(NetStashLogLevel.Information, message, null, values);
        }

        /// <summary>
        /// Registra um novo log com nivel <see cref="NetStashLogLevel.Warning"/>
        /// </summary>
        /// <param name="message">Mensagem descritiva do log</param>
        /// <param name="values">Valores adicionais que serao incluidos no log</param>
        public void Warning(string message, Dictionary<string, string> values = null)
        {
            Log(NetStashLogLevel.Warning, message, null, values);
        }

        /// <summary>
        /// Registra um novo log com nivel <see cref="NetStashLogLevel.Error"/>
        /// </summary>
        /// <param name="message">Mensagem descritiva do log</param>
        /// <param name="values">Valores adicionais que serao incluidos no log</param>
        internal void InternalError(string message, Dictionary<string, string> values = null)
        {
            Log(NetStashLogLevel.Error, message, null, values);
        }

        /// <summary>
        /// Registra um novo log com nivel <see cref="NetStashLogLevel.Fatal"/>
        /// </summary>
        /// <param name="exception">Exceçao que sera anexado ao log</param>
        /// <param name="values">Valores adicionais que serao incluidos no log</param>
        public void Error(Exception exception, Dictionary<string, string> values)
        {
            Log(NetStashLogLevel.Error, exception.Message, exception, values);
        }

        /// <summary>
        /// Registra um novo log com nivel <see cref="NetStashLogLevel.Error"/>
        /// </summary>
        /// <param name="message">Mensagem descritiva do log</param>
        /// <param name="values">Valores adicionais que serao incluidos no log</param>
        public void Error(string message, Dictionary<string, string> values = null)
        {
            Log(NetStashLogLevel.Error, message, null, values);
        }

        /// <summary>
        /// Registra um novo log com nivel <see cref="NetStashLogLevel.Error"/>
        /// </summary>
        /// <param name="message">Mensagem descritiva do log</param>
        /// <param name="exception">Exceçao que sera anexado ao log</param>
        /// <param name="values">Valores adicionais que serao incluidos no log</param>
        public void Error(string message, Exception exception, Dictionary<string, string> values = null)
        {
            Log(NetStashLogLevel.Error, message, exception, values);
        }

        /// <summary>
        /// Registra um novo log com nivel <see cref="NetStashLogLevel.Fatal"/>
        /// </summary>
        /// <param name="message">Mensagem descritiva do log</param>
        /// <param name="values">Valores adicionais que serao incluidos no log</param>
        public void Fatal(string message, Dictionary<string, string> values = null)
        {
            Log(NetStashLogLevel.Fatal, message, null, values);
        }

        /// <summary>
        /// Registra um novo log com nivel <see cref="NetStashLogLevel.Fatal"/>
        /// </summary>
        /// <param name="message">Mensagem descritiva do log</param>
        /// <param name="exception">Exceçao que sera anexado ao log</param>
        /// <param name="values">Valores adicionais que serao incluidos no log</param>
        public void Fatal(string message, Exception exception, Dictionary<string, string> values = null)
        {
            Log(NetStashLogLevel.Fatal, message, exception, values);
        }

        /// <summary>
        /// Registra um novo evento de log
        /// </summary>
        /// <param name="level">Nivel do log</param>
        /// <param name="message">Mensagem descritiva do log</param>
        /// <param name="exception">Exceçao que sera logado</param>
        /// <param name="addtionalValues">Valores adicionais que serao incluidos no log</param>
        public void Log(NetStashLogLevel level, string message, Exception exception,
            Dictionary<string, string> addtionalValues)
        {
            var netStashEvent = new NetStashEvent
            {
                Level = level.ToString(),
                Message = message,
                ExceptionMessage = exception?.Message,
                ExceptionDetails = exception?.StackTrace,
                Fields = addtionalValues
            };

            AddSendToLogstash(netStashEvent);
        }
        
        private void AddSendToLogstash(NetStashEvent e, bool run = true)
        {
            e.Machine = Environment.MachineName;
            e.Source = _system;
            e.Index = _logger;

            Storage.Proxy.LogProxy proxy = new Storage.Proxy.LogProxy();
            proxy.Add(e);

            if (run)
                TcpWorker.Run();
        }
    }
}
