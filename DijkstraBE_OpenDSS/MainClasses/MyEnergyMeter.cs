using System.Collections.Generic;

namespace ExecutorOpenDSS.Classes_Auxiliares
{
    class MyEnergyMeter
    {
        public double MaxkW = 0;
        public double MaxkWLosses = 0;
        public double KWh = 0;
        public double kvarh = 0;
        public double LossesKWh = 0;
        public double TransformerLosses = 0;
        public double BTLineLosses = 0;
        public double lineLossesPosMode = 0;
        public double lineLossesZeroMode = 0;
        public double NoLoadLosseskWh = 0;
        public double MTEnergy = 0;
        public double BTEnergy = 0;
        public string _sMes = "0";
        public double MTLineLosses34KV = 0;
        public double MTLineLosses = 0;
        public double TransformerAllLosses34KV = 0;
        public double KWhGD = 0;
        public double loadMultAlim = 1;

        // Construtor por copia
        public MyEnergyMeter(MyEnergyMeter em)
        {
            KWh = em.KWh;
            kvarh = em.kvarh;
            LossesKWh = em.LossesKWh;
            TransformerLosses = em.TransformerLosses;
            MTLineLosses = em.MTLineLosses;
            BTLineLosses = em.BTLineLosses;
            lineLossesPosMode = em.lineLossesPosMode;
            lineLossesZeroMode = em.lineLossesZeroMode;
            NoLoadLosseskWh = em.NoLoadLosseskWh;
            MTEnergy = em.MTEnergy;
            BTEnergy = em.BTEnergy;
            _sMes = em._sMes;
            MTLineLosses34KV = em.MTLineLosses34KV;
            TransformerAllLosses34KV  = em.TransformerAllLosses34KV;
            KWhGD = em.KWhGD;
            loadMultAlim = em.loadMultAlim;
        }

        public void GravaLoadMult(double LM)
        {
            loadMultAlim = LM;
        }

        // Construtor por lista
        public MyEnergyMeter(List<double> perdasMax, List<double> perdasEnergia)
        {
            MaxkW = perdasMax[0];
            MaxkWLosses = perdasMax[1];

            PreencheEnergia(perdasEnergia);
        }

        public MyEnergyMeter()
        {
        }

        public void GeracaoPerdasPotEPerdasEnergiaType(List<double> _geracaoPerdasPotEPerdasEnergia)
        {
            MaxkW = _geracaoPerdasPotEPerdasEnergia[0];
            MaxkWLosses = _geracaoPerdasPotEPerdasEnergia[1];
            KWh = _geracaoPerdasPotEPerdasEnergia[2];
            kvarh = _geracaoPerdasPotEPerdasEnergia[3];
            LossesKWh = _geracaoPerdasPotEPerdasEnergia[4];
            TransformerLosses = _geracaoPerdasPotEPerdasEnergia[5];
            MTLineLosses = _geracaoPerdasPotEPerdasEnergia[6];
            BTLineLosses = _geracaoPerdasPotEPerdasEnergia[7];
            lineLossesPosMode = _geracaoPerdasPotEPerdasEnergia[8];
            lineLossesZeroMode = _geracaoPerdasPotEPerdasEnergia[9];
            NoLoadLosseskWh = _geracaoPerdasPotEPerdasEnergia[10];
            MTEnergy = _geracaoPerdasPotEPerdasEnergia[11];
            BTEnergy = _geracaoPerdasPotEPerdasEnergia[12];
            MTLineLosses34KV = _geracaoPerdasPotEPerdasEnergia[13];
            TransformerAllLosses34KV = _geracaoPerdasPotEPerdasEnergia[14];
        }

        // operador soma  de energia 
        public void Soma(MyEnergyMeter em)
        {
            KWh += em.KWh;
            kvarh += em.kvarh;
            LossesKWh += em.LossesKWh;
            TransformerLosses += em.TransformerLosses;
            MTLineLosses += em.MTLineLosses;
            BTLineLosses += em.BTLineLosses;
            lineLossesPosMode += em.lineLossesPosMode;
            lineLossesZeroMode += em.lineLossesZeroMode;
            NoLoadLosseskWh += em.NoLoadLosseskWh;
            MTEnergy += em.MTEnergy;
            BTEnergy += em.BTEnergy;
            MTLineLosses34KV += em.MTLineLosses34KV;
            TransformerAllLosses34KV += em.TransformerAllLosses34KV;
            KWhGD += em.KWhGD;
        }

        // preenche campos de energia somente
        internal void PreencheEnergia(List<double> injecaoEPerdasEnergia)
        {
            KWh = injecaoEPerdasEnergia[0];
            kvarh = injecaoEPerdasEnergia[1];
            LossesKWh = injecaoEPerdasEnergia[2];
            TransformerLosses = injecaoEPerdasEnergia[3];
            MTLineLosses = injecaoEPerdasEnergia[4];
            BTLineLosses = injecaoEPerdasEnergia[5];
            lineLossesPosMode = injecaoEPerdasEnergia[6];
            lineLossesZeroMode = injecaoEPerdasEnergia[7];
            NoLoadLosseskWh = injecaoEPerdasEnergia[8];
            MTEnergy = injecaoEPerdasEnergia[9];
            BTEnergy = injecaoEPerdasEnergia[10];
            MTLineLosses34KV = injecaoEPerdasEnergia[11];
            TransformerAllLosses34KV = injecaoEPerdasEnergia[12];
        }

        internal void MultiplicaEnergia(int numDias)
        {
            KWh = KWh * numDias;
            kvarh = kvarh * numDias ;
            LossesKWh = LossesKWh * numDias ;
            TransformerLosses = TransformerLosses * numDias;
            MTLineLosses =  MTLineLosses * numDias;
            BTLineLosses =  BTLineLosses * numDias;
            lineLossesPosMode = lineLossesPosMode * numDias;
            lineLossesZeroMode = lineLossesZeroMode * numDias;
            NoLoadLosseskWh = NoLoadLosseskWh * numDias;
            MTEnergy = MTEnergy * numDias;
            BTEnergy = BTEnergy * numDias;
            MTLineLosses34KV = MTLineLosses34KV * numDias;
            TransformerAllLosses34KV = TransformerAllLosses34KV * numDias;
            KWhGD = TransformerAllLosses34KV * numDias;
        }

        public string FormataResultado(string nomeAlim)
        {
            string conteudo ="";
            conteudo += nomeAlim + "\t";

            //Obtem medidores
            conteudo += MaxkW.ToString("0.0000") + "\t"; //MaxkW
            conteudo += MaxkWLosses.ToString("0.0000") + "\t"; //MaxkWLosses

            //Obtem medidores
            conteudo += KWh.ToString("0.0000") + "\t"; //kW
            conteudo += kvarh.ToString("0.0000") + "\t"; //kvarh
            conteudo += LossesKWh.ToString("0.0000") + "\t"; //LossesKWh

            //perdas em transformadores
            conteudo += TransformerLosses.ToString("0.0000") + "\t"; //TransformerLosses

            //perdas MT e BT
            conteudo += MTLineLosses.ToString("0.0000") + "\t"; //MTLosses
            conteudo += BTLineLosses.ToString("0.0000") + "\t"; //BTLosses

            //Line losses. Struct com os seguintes campos:
            conteudo += lineLossesPosMode.ToString("0.0000") + "\t"; //lineLossesPosMode
            conteudo += lineLossesZeroMode.ToString("0.0000") + "\t"; //lineLossesZeroMode

            // NoLoadLosses
            conteudo += NoLoadLosseskWh.ToString("0.0000") + "\t"; //NoLoadLosses

            // MT Energy
            conteudo += MTEnergy.ToString("0.0000") + "\t"; //

            // BT Energy
            conteudo += BTEnergy.ToString("0.0000") + "\t"; //

            //  MTLineLosses34KV
            conteudo += MTLineLosses34KV.ToString("0.0000") + "\t"; //

            // TransformerAllLosses34KV
            conteudo += TransformerAllLosses34KV.ToString("0.0000") + "\t"; //

            // mes
            conteudo += _sMes + "\t"; //

            //load mult
            conteudo += loadMultAlim.ToString("0.0000") + "\t";

            return conteudo;
        }
        
        // seta o mes do calculo
        public void SetMesEM(int iMes)
        {
            _sMes = iMes.ToString();
        }
    }
}
