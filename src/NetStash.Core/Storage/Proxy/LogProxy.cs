using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;

namespace NetStash.Core.Storage.Proxy
{
    public class LogProxy : BaseProxy
    {
        /// <summary>
        /// Adiciona uma nova mensagem no banco de dados
        /// </summary>
        /// <param name="log">Evento que sera adicionado</param>
        public void Add(NetStashEvent log)
        {
            Entities.Log addLog = new Entities.Log();
            addLog.Message = Newtonsoft.Json.JsonConvert.SerializeObject(log.ToDictionary());

            using (IDbConnection db = base.GetConnection())
            using (IDbCommand cmd = db.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Log (Message) VALUES (@Message)";
                cmd.CommandType = CommandType.Text;
                IDbDataParameter pMessage = cmd.CreateParameter();
                pMessage.ParameterName = "@Message";
                pMessage.Value = addLog.Message;
                cmd.Parameters.Add(pMessage);
                db.Open();
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Recupera um evento do banco de dados
        /// </summary>
        /// <param name="id">Codigo do evento</param>
        /// <returns></returns>
        public NetStashEvent Get(out long id)
        {
            Entities.Log getLog = null;
            using (IDbConnection db = GetConnection())
            using (IDbCommand cmd = db.CreateCommand())
            {
                cmd.CommandText = "SELECT IdLog, Message from Log order by IdLog asc LIMIT 1";
                cmd.CommandType = CommandType.Text;
                db.Open();
                cmd.Prepare();

                IDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    getLog = new Entities.Log
                    {
                        IdLog = reader.GetInt64(reader.GetOrdinal("IdLog")),
                        Message = reader.IsDBNull(reader.GetOrdinal("Message"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("Message"))
                    };

                }
            }

            if (getLog == null)
            {
                id = -1;
                return null;
            }

            id = getLog.IdLog;

            return JsonConvert.DeserializeObject<NetStashEvent>(getLog.Message);
        }

        /// <summary>
        /// Recupera uma lista de eventos
        /// </summary>
        /// <param name="count">Quantidade maxima de mensagens que devem ser retornadas</param>
        /// <returns></returns>
        public Dictionary<long, string> GetList(int count = 100)
        {
            Dictionary<long, string> ret = new Dictionary<long, string>();

            List<Entities.Log> evs = new List<Entities.Log>();

            using (IDbConnection db = base.GetConnection())
            using (IDbCommand cmd = db.CreateCommand())
            {
                cmd.CommandText = "SELECT IdLog, Message from Log order by IdLog asc LIMIT " + count;
                cmd.CommandType = CommandType.Text;
                db.Open();
                cmd.Prepare();

                IDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Entities.Log log = new Entities.Log();

                    log.IdLog = reader.GetInt64(reader.GetOrdinal("IdLog"));
                    log.Message = reader.IsDBNull(reader.GetOrdinal("Message")) ? null : reader.GetString(reader.GetOrdinal("Message"));

                    evs.Add(log);
                }
            }

            foreach (Entities.Log item in evs)
                ret.Add(item.IdLog, item.Message);

            return ret;
        }

        /// <summary>
        /// Exclui um evento do bano de daos
        /// </summary>
        /// <param name="id">Codigo do evento</param>
        public void Delete(long id)
        {
            if (id < 0) return;

            using (IDbConnection db = base.GetConnection())
            using (IDbCommand cmd = db.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Log WHERE IdLog = @IdLog";
                cmd.CommandType = CommandType.Text;
                IDbDataParameter pIdLog = cmd.CreateParameter();
                pIdLog.ParameterName = "@IdLog";
                pIdLog.Value = id;
                cmd.Parameters.Add(pIdLog);
                db.Open();
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
