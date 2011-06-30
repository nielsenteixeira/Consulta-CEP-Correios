using System.Collections.Generic;
using System.Web.Mvc;
using System.Text;

using HtmlAgilityPack;

namespace ConsultaCEP.Controllers
{
    public class CepController : Controller
    {
        public JsonResult Consulta(string cep)
        {
            string uri = ("http://m.correios.com.br/movel/buscaCepConfirma.do?cepEntrada=" + cep + "&tipoCep=&cepTemp=&metodo=buscarCep");

            HtmlWeb htWeb = new HtmlWeb();
            var retorno = htWeb.Load(uri);

            var form = retorno.GetElementbyId("frmCep");

            if (form == null)
                return Json(new { Mensagem = "Cep não encontrado!" }, JsonRequestBehavior.AllowGet);

            var result = new List<string>();

            foreach (var item in form.SelectNodes("//span[@class='respostadestaque']"))
                result.Add(item.InnerText.Trim().Replace("\n", "").Replace("\t", "").Replace("  ", ""));

            return Json(new
            {
                Logradouro = result[0],
                Bairro = result[1],
                Localidade = new
                {
                    Cidade = result[2].Split('/')[0],
                    Estado = result[2].Split('/')[1]
                },
                Cep = result[3],
                Mensagem = (result[3] != cep) ? "Cep Modificado" : "Sucesso"
            }, JsonRequestBehavior.AllowGet);
        }

    }
}