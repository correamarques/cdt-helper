using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
  public class FaturaService
  {
    public static void Run()
    {
      List<Fatura> faturas = FillFaturas();

      // verifico se existe alguma fechada e com o pagamento efetuardo
      if (faturas.Any(w => w.situacaoProcessamento == "FECHADA" && w.pagamentoEfetuado))
      {
        PrintInvoicesPaid(faturas);

        bool ultimaFaturaPaga = false;
        foreach (var fatura in faturas.OrderByDescending(o => o.dataFechamento))
        {
          if (fatura.pagamentoEfetuado)
            ultimaFaturaPaga = true;

          if (ultimaFaturaPaga)
            fatura.pagamentoEfetuado = true;
          Console.WriteLine(fatura.dataFechamento + " - paid: " + fatura.pagamentoEfetuado);
        }
        PrintInvoicesPaid(faturas);

        Console.ReadLine();
      }
    }

    static void PrintInvoicesPaid(List<Fatura> faturas)
    {
      Console.WriteLine("Faturas pagas: " + faturas.Count(w => w.situacaoProcessamento == "FECHADA" && w.pagamentoEfetuado));
    }

    private static List<Fatura> FillFaturas()
    {
      #region JSON faturas
      StringBuilder j = new StringBuilder();
      j.AppendLine("[{                                            ");
      j.AppendLine("\"idConta\": 468647,                          ");
      j.AppendLine("\"situacaoProcessamento\": \"ABERTA\",        ");
      j.AppendLine("\"pagamentoEfetuado\": false,                 ");
      j.AppendLine("\"dataVencimentoFatura\": \"2018-12-05\",     ");
      j.AppendLine("\"dataVencimentoReal\": \"2018-12-05\",       ");
      j.AppendLine("\"dataFechamento\": \"2018-11-19\",           ");
      j.AppendLine("\"valorTotal\": 42.34,                        ");
      j.AppendLine("\"valorPagamentoMinimo\": null,               ");
      j.AppendLine("\"saldoAnterior\": null                       ");
      j.AppendLine("},                                            ");
      j.AppendLine("{                                             ");
      j.AppendLine("\"idConta\": 468647,                          ");
      j.AppendLine("\"situacaoProcessamento\": \"ABERTA\",        ");
      j.AppendLine("\"pagamentoEfetuado\": false,                 ");
      j.AppendLine("\"dataVencimentoFatura\": \"2018-11-05\",     ");
      j.AppendLine("\"dataVencimentoReal\": \"2018-11-05\",       ");
      j.AppendLine("\"dataFechamento\":\"2018-10-18\",            ");
      j.AppendLine("\"valorTotal\": 1390.14,                      ");
      j.AppendLine("\"valorPagamentoMinimo\": null,               ");
      j.AppendLine("\"saldoAnterior\": null                       ");
      j.AppendLine("},                                            ");
      j.AppendLine("{                                             ");
      j.AppendLine("\"idConta\": 468647,                          ");
      j.AppendLine("\"situacaoProcessamento\": \"ABERTA\",        ");
      j.AppendLine("\"pagamentoEfetuado\": false,                 ");
      j.AppendLine("\"dataVencimentoFatura\": \"2018-10-05\",     ");
      j.AppendLine("\"dataVencimentoReal\": \"2018-10-05\",       ");
      j.AppendLine("\"dataFechamento\": \"2018-09-17\",           ");
      j.AppendLine("\"valorTotal\": 1390.14,                      ");
      j.AppendLine("\"valorPagamentoMinimo\": null,               ");
      j.AppendLine("\"saldoAnterior\": null                       ");
      j.AppendLine("},                                            ");
      j.AppendLine("{                                             ");
      j.AppendLine("\"idConta\": 468647,                          ");
      j.AppendLine("\"situacaoProcessamento\": \"ABERTA\",        ");
      j.AppendLine("\"pagamentoEfetuado\": false,                 ");
      j.AppendLine("\"dataVencimentoFatura\": \"2018-09-05\",     ");
      j.AppendLine("\"dataVencimentoReal\": \"2018-09-05\",       ");
      j.AppendLine("\"dataFechamento\": \"2018-08-20\",           ");
      j.AppendLine("\"valorTotal\": 3673.67,                      ");
      j.AppendLine("\"valorPagamentoMinimo\": null,               ");
      j.AppendLine("\"saldoAnterior\": 2063.93                    ");
      j.AppendLine("},                                            ");
      j.AppendLine("{                                             ");
      j.AppendLine("\"idConta\": 468647,                          ");
      j.AppendLine("\"situacaoProcessamento\": \"FECHADA\",       ");
      j.AppendLine("\"pagamentoEfetuado\": false,                 ");
      j.AppendLine("\"dataVencimentoFatura\": \"2018-08-05\",     ");
      j.AppendLine("\"dataVencimentoReal\": \"2018-08-08\",       ");
      j.AppendLine("\"dataFechamento\": \"2018-07-18\",           ");
      j.AppendLine("\"valorTotal\": 2063.93,                      ");
      j.AppendLine("\"valorPagamentoMinimo\": 2063.93,            ");
      j.AppendLine("\"saldoAnterior\": 6485.74                    ");
      j.AppendLine("},                                            ");
      j.AppendLine("{                                             ");
      j.AppendLine("\"idConta\": 468647,                          ");
      j.AppendLine("\"situacaoProcessamento\": \"FECHADA\",       ");
      j.AppendLine("\"pagamentoEfetuado\": true,                  ");
      j.AppendLine("\"dataVencimentoFatura\": \"2018-07-05\",     ");
      j.AppendLine("\"dataVencimentoReal\": \"2018-07-09\",       ");
      j.AppendLine("\"dataFechamento\": \"2018-06-18\",           ");
      j.AppendLine("\"valorTotal\": 6485.74,                      ");
      j.AppendLine("\"valorPagamentoMinimo\": 6485.74,            ");
      j.AppendLine("\"saldoAnterior\": 3230.19                    ");
      j.AppendLine("},                                            ");
      j.AppendLine("{                                             ");
      j.AppendLine("\"idConta\": 468647,                          ");
      j.AppendLine("\"situacaoProcessamento\": \"FECHADA\",       ");
      j.AppendLine("\"pagamentoEfetuado\": false,                 ");
      j.AppendLine("\"dataVencimentoFatura\": \"2018-06-05\",     ");
      j.AppendLine("\"dataVencimentoReal\": \"2018-06-07\",       ");
      j.AppendLine("\"dataFechamento\": \"2018-05-22\",           ");
      j.AppendLine("\"valorTotal\": 3230.19,                      ");
      j.AppendLine("\"valorPagamentoMinimo\": 1023.55,            ");
      j.AppendLine("\"saldoAnterior\": 1913.97                    ");
      j.AppendLine("},                                            ");
      j.AppendLine("{                                             ");
      j.AppendLine("\"idConta\": 468647,                          ");
      j.AppendLine("\"situacaoProcessamento\": \"FECHADA\",       ");
      j.AppendLine("\"pagamentoEfetuado\": true,                  ");
      j.AppendLine("\"dataVencimentoFatura\": \"2018-05-05\",     ");
      j.AppendLine("\"dataVencimentoReal\": \"2018-05-09\",       ");
      j.AppendLine("\"dataFechamento\": \"2018-04-19\",           ");
      j.AppendLine("\"valorTotal\": 1913.97,                      ");
      j.AppendLine("\"valorPagamentoMinimo\": 604.1,              ");
      j.AppendLine("\"saldoAnterior\": 947.82                     ");
      j.AppendLine("},                                            ");
      j.AppendLine("{                                             ");
      j.AppendLine("\"idConta\": 468647,                          ");
      j.AppendLine("\"situacaoProcessamento\": \"FECHADA\",       ");
      j.AppendLine("\"pagamentoEfetuado\": true,                  ");
      j.AppendLine("\"dataVencimentoFatura\": \"2018-04-05\",     ");
      j.AppendLine("\"dataVencimentoReal\": \"2018-04-09\",       ");
      j.AppendLine("\"dataFechamento\": \"2018-03-22\",           ");
      j.AppendLine("\"valorTotal\": 947.82,                       ");
      j.AppendLine("\"valorPagamentoMinimo\": 301.11,             ");
      j.AppendLine("\"saldoAnterior\": 5297.66                    ");
      j.AppendLine("},                                            ");
      j.AppendLine("{                                             ");
      j.AppendLine("\"idConta\": 468647,                          ");
      j.AppendLine("\"situacaoProcessamento\": \"FECHADA\",       ");
      j.AppendLine("\"pagamentoEfetuado\": true,                  ");
      j.AppendLine("\"dataVencimentoFatura\": \"2018-03-05\",     ");
      j.AppendLine("\"dataVencimentoReal\": \"2018-03-07\",       ");
      j.AppendLine("\"dataFechamento\": \"2018-02-19\",           ");
      j.AppendLine("\"valorTotal\": 5297.66,                      ");
      j.AppendLine("\"valorPagamentoMinimo\": 5297.66,            ");
      j.AppendLine("\"saldoAnterior\": 2935.88                    ");
      j.AppendLine("},                                            ");
      j.AppendLine("{                                             ");
      j.AppendLine("\"idConta\": 468647,                          ");
      j.AppendLine("\"situacaoProcessamento\": \"FECHADA\",       ");
      j.AppendLine("\"pagamentoEfetuado\": false,                 ");
      j.AppendLine("\"dataVencimentoFatura\": \"2018-02-05\",     ");
      j.AppendLine("\"dataVencimentoReal\": \"2018-02-07\",       ");
      j.AppendLine("\"dataFechamento\": \"2018-01-24\",           ");
      j.AppendLine("\"valorTotal\": 2935.88,                      ");
      j.AppendLine("\"valorPagamentoMinimo\": 901.71,             ");
      j.AppendLine("\"saldoAnterior\": 149.09                     ");
      j.AppendLine("},                                            ");
      j.AppendLine("{                                             ");
      j.AppendLine("\"idConta\": 468647,                          ");
      j.AppendLine("\"situacaoProcessamento\": \"FECHADA\",       ");
      j.AppendLine("\"pagamentoEfetuado\": true,                  ");
      j.AppendLine("\"dataVencimentoFatura\": \"2018-01-05\",     ");
      j.AppendLine("\"dataVencimentoReal\": \"2018-01-08\",       ");
      j.AppendLine("\"dataFechamento\": \"2017-12-18\",           ");
      j.AppendLine("\"valorTotal\": 149.09,                       ");
      j.AppendLine("\"valorPagamentoMinimo\": 60,                 ");
      j.AppendLine("\"saldoAnterior\": 0                          ");
      j.AppendLine("},                                            ");
      j.AppendLine("{                                             ");
      j.AppendLine("\"idConta\": 468647,                          ");
      j.AppendLine("\"situacaoProcessamento\": \"FECHADA\",       ");
      j.AppendLine("\"pagamentoEfetuado\": false,                 ");
      j.AppendLine("\"dataVencimentoFatura\": \"2017-12-05\",     ");
      j.AppendLine("\"dataVencimentoReal\": \"2017-12-07\",       ");
      j.AppendLine("\"dataFechamento\": \"2017-11-21\",           ");
      j.AppendLine("\"valorTotal\": 0,                            ");
      j.AppendLine("\"valorPagamentoMinimo\": 0,                  ");
      j.AppendLine("\"saldoAnterior\": 0                          ");
      j.AppendLine("}                                             ");
      j.AppendLine("]                                             ");
      #endregion

      List<Fatura> o = JsonConvert.DeserializeObject<List<Fatura>>(j.ToString());
      return o;
    }

    static List<Fatura> FillFaturas2()
    {
      List<Fatura> faturas = new List<Fatura>();
      faturas.Add(new Fatura() { dataFechamento = "2018-04-17", pagamentoEfetuado = true });

      return faturas;
    }

    class Fatura
    {
      public int idConta { get; set; }
      public string situacaoProcessamento { get; set; }
      public bool pagamentoEfetuado { get; set; }
      public string dataVencimentoFatura { get; set; }
      public string dataVencimentoReal { get; set; }
      public string dataFechamento { get; set; }
      public double valorTotal { get; set; }
      public double? valorPagamentoMinimo { get; set; }
      public double? saldoAnterior { get; set; }
    }

  }
}
