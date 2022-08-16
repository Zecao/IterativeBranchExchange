% gera populacao inicial Alim
function [populacao, fitness] = geraPopulacao(alim)

% alimentador Cemig  
if (strcmp(alim.Ftipo,'cemig'))
    
    % obtem interconexoes 
    lstChaveInterConexoes = obtemInterConexoesPorLst(alim.FlstAlimentadores);
    
    % constroi populacao Cemig
    [populacao, fitness] = constroiPopulacaoCemig(lstChaveInterConexoes,alim);
     
% alimentadores IEEE
else

    global paramAG;    
    
    populacao = [];
    
    % so busca local 
    if (paramAG.tamPopulacao ~= 1)
        % carrega populacao IEEE
        populacao = carregaPopulacaoIEEE(alim);
    
    end

    % TODO
    % adiciona individuo Original na populacao
    populacao = adicionaConfOriginal(alim,populacao);
    
    % Avalia a funcao objetiva para a nova populacao
    fitness = avaliaPopulacao(populacao, alim, []);
    
end

end

% carrega populacao IEEE
function populacao = carregaPopulacaoIEEE(alim)

switch alim.Fnome 
    
    case 'alim\Sistema_Zhu2002_3fontes.txt';

        % OBS; ha grande chance de que na populacao gerada ja exista o
        % minimo, como eh caso da populacao 'populacaoSis1.mat'
        load('populacaoSis1.mat');
  
    case 'alim\Sistema_Zhu2002.txt';
        
        load('populacaoSis2.mat');
            
    case 'alim\Sistema_Huang2002.txt'
        
        load('populacaoSis3.mat');

    case 'alim\Sistema_Brasileiro2008.txt';
         
        % arquivo populacao (ja convertido)
        load('populacaoSis4.mat');
        
    case 'alim\Sistema_Brasileiro2008_teste';
         
        % arquivo populacao (ja convertido)
        load('populacaoSis4.mat');
      
    case 'alim\Sistema_TPC2003.txt';
           
        load('populacaoSis5.mat'); 
          
    case 'alim\Sistema_415.txt';
        
        load('populacaoSis6.mat'); 
        
    case 'Sistema119buses';
        
        load('populacaoSis7.mat'); 
        
    otherwise
    
        %gera populacao utilizando o algoritmo do Carlos
        aleatorio = true; %default 
        populacao = gerapopCarlos(alim, aleatorio);
            
end

end

% adiciona individuo Original na populacao
function populacao = adicionaConfOriginal(alim,populacao)

% adiciona configuracao atual
indOriginal = chaves2individuo(alim.FmChavesIEEE);

% SE indOriginal NAO eh membro
if (~ismember(indOriginal,populacao))

    % adiciona individuo configuracao original na populacao
    populacao = [indOriginal; populacao];
    
end

% limita tamanho populacao
global paramAG;
populacao = populacao(1:paramAG.tamPopulacao,:);

end
