using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExecutorOpenDSS
{
    class Configuracoes
    {
        //Lê o arquivo XML com as configurações
        public static bool GetConfiguracoes(MainWindow janela, string arquivo)
        {
            //Verifica se o arquivo existe
            if(File.Exists(arquivo)) 
            {
                try
                {
                    //Lê o XML
                    XDocument xDoc = XDocument.Load(arquivo);
                    
                    //Pega o elemento root
                    XElement raiz = xDoc.Root;
                   
                    //Define o caminho
                    janela.caminhoDSSTextBox.Text = raiz.Element("CaminhoMensais").Value;

                    //Define o caminho recursos permanentes
                    janela.caminhoPermTextBox.Text = raiz.Element("CaminhoRecPerm").Value;                    
                   
                    //Define o tipo de fluxo
                    janela.tipoFluxoComboBox.Text = raiz.Element("TipoFluxo").Value;
                   
                    //Define o tipo de dia
                    janela.tipoDiaComboBox.Text = raiz.Element("TipoDia").Value;

                    //Define o mês
                    janela.mesComboBox.Text = raiz.Element("Mes").Value;

                    //Define o ano
                    janela.anoTextBox.Text = raiz.Element("Ano").Value;

                    //Define checkbox Otimiza LoadMult
                    janela.otimizaEnergiaCheckBox.IsChecked = Convert.ToBoolean(raiz.Element("OtmEnergiaChecked").Value);

                    //Define checkbox Otimiza Energia 
                    janela.otimizaCheckBox.IsChecked = Convert.ToBoolean( raiz.Element("OtmChecked").Value );                                      
                                                        
                    //Define o incremento do ajuste
                    janela.incrementoAjusteTextBox.Text = raiz.Element("IncrementoAjuste").Value;                                   
                                        
                    //Define o check-box usar tensao barramento
                    janela.usarTensoesBarramentoCheckBox.IsChecked = Convert.ToBoolean(raiz.Element("UsarTensaoBar").Value);

                    //Define o TextBox tensaoPU default
                    janela.tensaoSaidaBarTextBox.Text = raiz.Element("TensaoPUDefault").Value;

                    //Define o check-box usar tensao barramento
                    janela.fluxoDiarioNativoCheckBox.IsChecked = Convert.ToBoolean(raiz.Element("FluxoDiarioNativo").Value);

                    //Adiciona o loadMult alternativo
                    janela.loadMultAltTextBox.Text = raiz.Element("loadMultAlternativo").Value;

                    //Define o check-box  _calcDRPDRC
                    janela._parAvan._calcDRPDRC = Convert.ToBoolean(raiz.Element("CalculaDRPDRC").Value);

                    //Define o check-box  _calcTensaoBarTrafo
                    janela._parAvan._calcTensaoBarTrafo = Convert.ToBoolean(raiz.Element("CalcTensaoBarTrafo").Value);

                    //Define o check-box  _calcTensaoBarTrafo
                    janela._parAvan._verifCargaIsolada = Convert.ToBoolean(raiz.Element("VerifCargaIsolada").Value);
                    
                    //Define o check-box  calculaPUOtm
                    janela._parAvan._otimizaPUSaidaSE  = Convert.ToBoolean(raiz.Element("CalculaPUOtm").Value);
                    
                    //Retorna verdadeiro se tudo ocorreu bem
                    return true;
                }
                catch
                {
                    // TODO exibir excecao 
                    //Caso haja algum problema na leitura, retorna falso
                    return false;
                }
                
            }
            else
            {
                //Retorna falso caso o arquivo não exista
                return false;
            }
        }

        //Grava o arquivo XML com as configurações
        public static void SetConfiguracoes(MainWindow janela, string arquivo)
        {
            //Cria um novo arquivo xml
            XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"));
            
            //Cria o elemento root
            XElement raiz = new XElement("Configuracoes");
            
            //Adiciona o caminho
            raiz.Add(new XElement("CaminhoMensais", janela.caminhoDSSTextBox.Text));

            //Adiciona o caminho
            raiz.Add(new XElement("CaminhoRecPerm", janela.caminhoPermTextBox.Text));

            //Adiciona o mês
            raiz.Add(new XElement("Mes", janela.mesComboBox.Text));

            //Adiciona o mês
            raiz.Add(new XElement("Ano", janela.anoTextBox.Text));
            
            //Adiciona se deve permitir mensagens de erro do OpenDSS
            raiz.Add(new XElement("AllowForms", janela.AllowFormsCheckBox.IsChecked.Value));
            
            //Adiciona o tipo de fluxo
            raiz.Add(new XElement("TipoFluxo", janela.tipoFluxoComboBox.Text));
            
            //Adiciona o tipo de dia
            raiz.Add(new XElement("TipoDia", janela.tipoDiaComboBox.Text));

            //Adiciona se a otimização está habilitada
            raiz.Add(new XElement("OtmChecked", janela.otimizaCheckBox.IsChecked.Value));

            //Adiciona se a otimização energia está habilitada
            raiz.Add(new XElement("OtmEnergiaChecked", janela.otimizaEnergiaCheckBox.IsChecked.Value));

            //Adiciona o incremento do ajuste
            raiz.Add(new XElement("IncrementoAjuste", janela.incrementoAjusteTextBox.Text));

            //Adiciona o usarTensoesBarramentoCheckBox
            raiz.Add(new XElement("UsarTensaoBar", janela.usarTensoesBarramentoCheckBox.IsChecked.Value));

            //Adiciona a tensaoPU default
            raiz.Add(new XElement("TensaoPUDefault", janela.tensaoSaidaBarTextBox.Text));

            //Adiciona o usarTensoesBarramentoCheckBox
            raiz.Add(new XElement("FluxoDiarioNativo", janela.fluxoDiarioNativoCheckBox.IsChecked.Value));

            //Adiciona o loadMult alternativo
            raiz.Add(new XElement("loadMultAlternativo", janela.loadMultAltTextBox.Text));

            //Adiciona o calculaDRPDRCCheckBox
            raiz.Add(new XElement("CalculaDRPDRC", janela._parAvan._calcDRPDRC));

            //Adiciona o loadMult alternativo
            raiz.Add(new XElement("CalculaPUOtm", janela._parAvan._otimizaPUSaidaSE ));

            //Adiciona o loadMult _calcTensaoBarTrafo
            raiz.Add(new XElement("CalcTensaoBarTrafo", janela._parAvan._calcTensaoBarTrafo));

            //Adiciona o loadMult _verifCargaIsolada
            raiz.Add(new XElement("VerifCargaIsolada", janela._parAvan._verifCargaIsolada));

            //Adiciona o root ao XML
            xDoc.Add(raiz);

            //SAlva o arquivo XML
            xDoc.Save(arquivo);
        }
    }
}
