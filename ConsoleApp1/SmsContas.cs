using System;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
  public class SmsContas
  {
    #region Counts without SMS enabled
    //select count(1)
    //from contas c
    //JOIN Produtos p(NOLOCK)
    //ON p.ID_PRODUTO = c.ID_PRODUTO
    //	INNER JOIN ParametrosProdutos pp
    //ON pp.ID_PRODUTO = P.ID_PRODUTO
    //AND pp.CODIGO = 'HabilitaSmsControle' AND pp.Logico = 1
    //where 1=1
    //and status not in (
    //	4, -- Cancelado Glosa
    //	8, -- Creliq
    //	9, -- Perda
    //	14, -- Cancelada Falec. Titular
    //	15, -- Acordo Creliq
    //	31, -- Cancelado Desaverbado
    //	34, -- Cancelado Definitivo - Fraude
    //	201, -- Cancelado Definitivo - PLD
    //	202 -- Conta Duplicada
    //  )
    //and not exists(
    //	select id_conta
    //	from SMSContas s
    //	where c.id_conta = s.id_conta
    //	and s.id_smstiposervico = 1
    //)
    #endregion

    static StringBuilder query = new StringBuilder();
    //static readonly int INSERTS_PER_FILE = 3;
    static readonly int INSERTS_PER_FILE = 50000;
    static readonly int TIPO_SERVICO = 1;

    public static void Run()
    {
      int row = 0, fileGenerated = 1;
      //string[] lines = System.IO.File.ReadAllLines(@"C:\Temp\sms_controle_contas.rpt").Take(9).ToArray(); // require using System.Linq;
      string[] lines = System.IO.File.ReadAllLines(@"C:\Temp\sms_controle_contas.rpt");

      while (row < lines.Length)
      {
        // New file, let's clear stringbuilder
        query.Clear();
        query.AppendLine("SET NOCOUNT ON");
        query.AppendLine("DECLARE @MessageComplement VARCHAR(140)");
        query.AppendLine("DECLARE @CadastrarMensagemRetorno INT");

        // first of all lets disable all things
        EnableOrDisableReferences(isReferencesEnabled: false);

        query.AppendLine();

        for (int i = 0; i < INSERTS_PER_FILE; i++)
        {
          if (row == lines.Length) // EOF = End Of File
            break;

          CreateInsertCommand(lines[row]);
          row++; 
        }

        // finally enable all things
        EnableOrDisableReferences(isReferencesEnabled: true);
        
        // save the result to file
        string fileName = string.Format("INSERT_SmsContas_TipoServico_{0}_File_{1}.sql", TIPO_SERVICO, fileGenerated.ToString().PadLeft(2, '0'));
        string folder = string.Format(@"C:\Temp\sms_contas\tipo_servico_{0}\", TIPO_SERVICO);
        System.IO.File.WriteAllText(string.Concat(folder, fileName), query.ToString(), Encoding.ASCII);

        Console.WriteLine("times executed: " + fileGenerated);
        fileGenerated++;
      }
    }

    static void CreateInsertCommand(string idConta)
    {
      // Verifica se a conta ecziste na tabela SmsContas
      AccountHasSmsEnabled(idConta);
      
      // Abre thread para relizar o cadastro
      query.AppendLine("\tBEGIN");

      // Non ecziste? Então realiza o insert
      InsertAccountInSmsContas(idConta);

      // Cria o complemento da mensagem
      GenerateMessageComplement(idConta);

      // Chama a proc para cadastrar a mensagem para a conta
      RegisterMessage(idConta);

      // Finaliza thread para relizar o cadastro
      query.AppendLine("\tEND");
    }

    static void AccountHasSmsEnabled(string idConta)
    {
      query.Append("IF NOT EXISTS(SELECT Id_Conta FROM SMSContas ");
      query.AppendFormat("WHERE Id_Conta = {0} ", idConta);
      query.AppendFormat("AND ID_SMSTipoServico = {0})", TIPO_SERVICO);
      query.AppendLine();
    }

    static void InsertAccountInSmsContas(string idConta)
    {
      query.Append("\t\tINSERT INTO SMSContas (Id_Conta, FlagAtivo, DataAtivacao, DataCancelamento, Origem, ID_SMSTipoServico, id_loginAtivacao, ");
      query.Append("id_loginCancelamento, Id_PlataformaAtivacao, Id_PlataformaCancelamento) VALUES(");
      query.AppendFormat("{0},", idConta);
      query.Append("1, ");                        // FlagAtivo
      query.Append("GETDATE(), ");                // DataAtivacao
      query.Append("NULL, ");                     // DataCancelamento
      query.Append("'SCRIPT', ");                 // Origem
      query.AppendFormat("{0}, ", TIPO_SERVICO);  // ID_SMSTipoServico
      query.Append("1, ");                        // id_loginAtivacao
      query.Append("NULL, ");                     // id_loginCancelamento
      query.Append("1, ");                        // Id_PlataformaAtivacao
      query.Append("NULL)");                      // Id_PlataformaCancelamento
      query.AppendLine();
    }

    static void GenerateMessageComplement(string idConta)
    {
      query.AppendFormat("\t\tSET @MessageComplement = '{0}' ", idConta);
      query.AppendFormat("+ CAST(dbo.FC_Calcula_DACModulo10({0}) AS VARCHAR(140))", idConta);
      query.AppendLine();
    }

    static void RegisterMessage(string idConta)
    {
      query.Append("\t\tEXEC @CadastrarMensagemRetorno = SPR_SMSCadastrarMensagem ");
      query.AppendFormat("@IdConta = {0}, ", idConta);
      query.Append("@IdOperacao = -2, ");
      query.Append("@Origem = 'SCRIPT', ");
      query.Append("@IdStatus = -1, ");
      query.Append("@Estabelecimento = NULL, ");
      query.Append("@ValorCompra = NULL, ");
      query.Append("@TipoSMS = 'ATIVACAO', ");
      query.Append("@HoraEnvio = NULL, ");
      query.Append("@ComplementoMensagem = @MessageComplement");
      query.AppendLine();
    }

    static void EnableOrDisableReferences(bool isReferencesEnabled)
    {
      query.AppendLine();
      query.AppendLine(string.Format("-- {0} todas as referencias", isReferencesEnabled ? "Habilitando" : "Desabilitando"));
      query.AppendLine(Trigger(isReferencesEnabled));
      query.AppendLine(Constraint(isReferencesEnabled));
    }

    static string Constraint(bool create)
    {
      if (create)
        return "ALTER TABLE [dbo].[SMSContas] ADD CONSTRAINT [DF_SMSContas_FlagAtivo] DEFAULT (0) FOR [FlagAtivo]";
      return "ALTER TABLE [dbo].[SMSContas] DROP CONSTRAINT [DF_SMSContas_FlagAtivo]";
    }

    static string Trigger(bool enable)
    {
      if (enable)
        return "ALTER TABLE [dbo].[SMSContas] ENABLE TRIGGER ALL";
      return "ALTER TABLE [dbo].[SMSContas] DISABLE TRIGGER ALL";
    }
  }
}
