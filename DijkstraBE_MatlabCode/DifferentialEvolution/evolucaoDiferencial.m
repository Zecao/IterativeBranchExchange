% roda Evolucao Diferencial de acordo com o tipo do algoritmo
function [populacao, fitness] = evolucaoDiferencial(algoritmo, alim, populacao, fitness, k)

switch algoritmo
    
    case 1 % ED Original / Cod. bin 

        [populacao, fitness] = evolucaoDiferencialOriginal(alim, populacao, fitness); 

    case 12 % ED Original / Cod. bin / BL(mutacao obbij) / Busca Local(min ciclos)

        [ populacao, fitness ] = ED_BuscaLocal12(alim, populacao, fitness, k);

    case 22 % ED Discreta(Lst Mov) / Cod. TS / Busca Local(min ciclos)

        [ populacao, fitness ] = evolucaoDiferencialDiscretaTS_MC(alim, populacao, fitness, k);    

    case 3 % AG classico
        
        [populacao, fitness] = classicGeneticAlg(populacao, fitness, alim, k);
        
    case 50 % ED Original - Cemig / Cod.Int

        [populacao, fitness] = evolucaoDiferencialOriginalCodTieSwCemig(alim, populacao, fitness);
    
    % TODO: reescrever para individuos cemig. 
    % o problema ocorre na function chaveMP = getLacoSP(chave, indBinOriginal, alim)
    % onde erradamente FECHO a chave baseado no indice da FmChavesIEEE 
    case 502 % ED Original - Cemig / Cod.Int/ Busca Local(min ciclos) 

        [populacao, fitness] = evolucaoDiferencialOriginalCodTieSwLaco_Cemig(alim, populacao, fitness, k);

    case 52 % ED / Cod.Int/ BL REDE 4 / selecaoOriginal - SEM BIJ e cruzamentoU       

        [populacao, fitness] = evolucaoDiferencialElite52(alim, populacao, fitness, k);    
        
    case 521 % ED / So busca local    

        [populacao, fitness] = evolucaoDiferencialElite521(alim, populacao, fitness, k);  
        
    case 55 % ED Discreta(Lst Mov)

        [populacao, fitness] = evolucaoDiferencialDiscretaCodTieSwLstMovLaco(alim, populacao, fitness, k); 

    case 6 % ED Discreta(Lst Mov) / BL(mutacao obbij). OK

        [populacao, fitness] = evolucaoDiferencialDiscretaMigracao(alim, populacao, fitness); 

    case 62 % ED Discreta(Lst Mov) / BL(mutacao obbij) / Busca Local(min ciclos). OK

        [populacao, fitness] = evolucaoDiferencialDiscretaCodBinMigracaoLaco(alim, populacao, fitness, k); 

    case 63 % 63 Algortimo 62 sem MC

        [populacao, fitness] = evolucaoDiferencialDiscretaCodBinOBBIJ63(alim, populacao, fitness);

    case 64 % 64 Algoritmo 63 sem MC e sem BIJ.

        [populacao, fitness] = evolucaoDiferencialDiscretaCodBin64(alim, populacao, fitness);

    otherwise
     
end

end