using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Xml;
using System.Dynamic;

using HtmlAgilityPack;

namespace ConsultaCEP.Controllers
{
    public class CepController : Controller
    {
        public JsonResult Consulta(string cep)
        {
            var sb = new StringBuilder();

            var buf = new byte[8192];

            Uri uri = new Uri("http://m.correios.com.br/movel/buscaCepConfirma.do?cepEntrada=" + cep +"&tipoCep=&cepTemp=&metodo=buscarCep");

            HttpWebRequest request = (HttpWebRequest)
             WebRequest.CreateDefault(uri);
            request.Method = "POST";
            
            HttpWebResponse response = (HttpWebResponse)
             request.GetResponse();

            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;

            do
            {
                count = resStream.Read(buf, 0, buf.Length);

                if (count != 0)
                {
                    tempString = Encoding.UTF8.GetString(buf, 0, count);
                    sb.Append(tempString);
                }
            }
            while (count > 0);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(sb.ToString());
            
            var form = doc.GetElementbyId("frmCep");

            if (form == null) 
                return Json(new { Mensagem = "Cep não encontrado!" }, JsonRequestBehavior.AllowGet);

            var elementos = form.SelectSingleNode("//div[@class='caixacampobranco']");
            
            var result = new List<string>();

            foreach (var item in elementos.SelectNodes("//span[@class='respostadestaque']"))
                result.Add(item.InnerText.Trim().Replace("\n", "").Replace("\t", "").Replace("  ", ""));

           return Json(new { 
                Logradouro = result[0],
                Bairro = result[1],
                Localidade = new {
                    Cidade = result[2].Split('/')[0], 
                    Estado = result[2].Split('/')[1]
                },
                Cep = result[3],
                Mensagem  = (result[3] != cep) ? "Cep Modificado" : "Sucesso"
            }, JsonRequestBehavior.AllowGet);
        }

    }
}