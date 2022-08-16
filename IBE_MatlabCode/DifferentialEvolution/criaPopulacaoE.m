% cria Populacao E
function efinal = criaPopulacaoE(populacao,alim)

% 2019
popOri = populacao;

% inicializacao parametros while
cont = 0;

% vetor de logicals que representam a radialidade dos individuos
radialBoolPopElites = false(size(populacao,1),1) ;

% percentual de individuoas radiais
global paramAG
percentualIndRadiaisElite = 0;

% novas populacoes
e = []; %populacao trial elites
efinal = [];

% mutacaoDE
% enquanto nao gerar individuos radiais
while ( percentualIndRadiaisElite < paramAG.IIR )    
    
    e = mutacaoDEOriginalCodTieSwElite(radialBoolPopElites,populacao);

    % OBS: operacoes adicionais ao "DE original"
    % 1. arredonda individuo
    e = arredondaPopulacaoTS(e, alim);
    
    % retorna p/ codificacao binaria
    ebin = tieSwitch2binario(e, alim); 

    % verifica radialidade
    radialBoolPopElites = verificaRadialidadeBGL(alim, ebin); 
   
    % 2018 cria populacao ufinal com os individuos radiais da populacao u
    efinal = [efinal; e(radialBoolPopElites,:)];
        
    %  atualiza indices while
    percentualIndRadiaisElite = size(efinal,1)/size(popOri,1)*100;
    
    % OBS: so vale para mutacaoDEOriginalCodTieSwElite
    % altera taxa de cruzamento Cr. 
%     atualizaTaxaCruzamento();
    
    % contador loops while
    if (cont == paramAG.iterIIR)
        break;
    else
        cont = cont + 1;
    end

end

% 2018
% se populacao u maior que populcao inicial
if ( size(efinal,1) > size(popOri,1) )

    efinal = efinal(1:size(popOri,1),:);
    
end

% 2019
% OBS: necessario incluir para evitar fim da diversidade populacional
efinal = unique(efinal,'rows');

end
