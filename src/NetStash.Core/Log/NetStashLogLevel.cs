namespace NetStash.Core.Log
{
    public enum NetStashLogLevel
    {
        /// <summary>
        /// Qualquer coisa e tudo que você pode querer saber sobre um bloco de código em execução.
        /// </summary>
         Verbose = 0,

        /// <summary>
        /// Eventos internos do sistema que não são necessariamente observáveis de fora.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// A força vital da inteligência operacional - as coisas acontecem.
        /// </summary>
        Information = 2,

        /// <summary>
        /// O serviço está degradado ou em perigo.
        /// </summary>
        Warning = 3,
        
        /// <summary>
        /// Funcionalidade não está disponível, dados perdidos ou quebrados.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Erro critico
        /// </summary>
        Fatal = 5
    }
}
