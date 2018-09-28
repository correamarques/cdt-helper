using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
  class Conta
  {
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public bool Duplicated { get; set; }
  }

  public class LimitesDisponibilidades
  {
    readonly string content, filePath = string.Empty;

    public string Content => content;
    public string GeneratedFile { get; set; }


    public LimitesDisponibilidades(string content, string filePath)
    {
      this.content = content;
      this.filePath = filePath;
    }


    public static void Run()
    {
      string filePath = @"C:\Temp\ajuste_contas5.txt";
      string content = System.IO.File.ReadAllText(filePath);

      LimitesDisponibilidades limitesDisponibilidades = new LimitesDisponibilidades(content, filePath);
      limitesDisponibilidades.UpdateLimitesDisponibilidades();
    }


    public string UpdateLimitesDisponibilidades()
    {
      System.IO.FileInfo fi = new System.IO.FileInfo(filePath);

      StringBuilder builder = new StringBuilder();
      List<Conta> contas = new List<Conta>();

      builder.AppendLine("SET NOCOUNT ON");
      builder.AppendLine("BEGIN");

      string[] lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

      foreach (var line in lines)
      {
        if (line.Length <= 5)
          break;

        int idConta = Int32.Parse(line.Split('\t')[0]);
        string value = line.Split('\t')[1];

        decimal amount = 0;
        if (value.IndexOf("R$") > 0)
          amount = Decimal.Parse(value.Replace("R$", ""));
        else
          amount = Decimal.Parse(value);

        contas.Add(new Conta() { Id = idConta, Amount = amount });
      }

      // busca todas as contas que estão duplicadas no arquivo
      foreach (var item in contas.GroupBy(c => c.Id).Where(grp => grp.Count() > 1).Select(grp => grp.Key))
      {// marca todas as contas como duplicada
        foreach (var conta in contas.Where(c => c.Id == item))
          conta.Duplicated = true;
      }

      // informa que o arquivo contem contas duplicadas
      if (contas.Where(w => w.Duplicated).Count() > 0)
        builder.AppendLine("\t--Arquivo com contas duplicadas (ContaDuplicada).");

      foreach (var conta in contas.OrderBy(w => w.Id))
      {
        builder.Append("\tUPDATE LimitesDisponibilidades ");
        builder.AppendFormat("SET DisponibGlobalCredito = DisponibGlobalCredito - {0}\t", conta.Amount.ToString(new CultureInfo("en-US")));
        // alinhar o where qdo tem valor baixo
        if (conta.Amount.ToString().Length <= 4) builder.Append("\t");
        builder.AppendFormat("WHERE Id_Conta = {0}", conta.Id);
        // adiciona um comentario no arquivo para facilitar encontrar as contas que estão duplicadas
        if (conta.Duplicated) builder.Append("\t -- ContaDuplicada");
        builder.AppendLine();
      }

      builder.AppendLine("END");

      GeneratedFile = fi.FullName.Replace(fi.Extension, ".sql");
      System.IO.File.WriteAllText(GeneratedFile, builder.ToString(), Encoding.ASCII);

      return builder.ToString();
    }
  }
}
