%
function [arrayStructElite, populacao, fitness, k, numMigracoes] = algGenDEFluxo( alim, algoritmo, sistema )

% gera populacao inicial Alim
[populacao, fitness] = geraPopulacao(alim);

% parametros AG
global paramAG;    
global param;
    
% inicio variaveis while
k = 1; % contador de geracoes
numMigracoes = 0; % contador de migracoes 

% cria struct elite 
arrayStructElite = criaArrayStructElite(paramAG.numGeracoes,alim);

% repete o numero de geracoes
while ( k <= paramAG.numGeracoes )

    % condicao de aborto (populacao sem diversiade)
    if (param.Abortar==1)
        break;
    end
    
    % procedimento de migracao % verifica diversidade populacional
    if (paramAG.migracaoBool) && (verificaDiversidadePopulacional(arrayStructElite, k))
                
        [populacao, fitness, numMigracoes] = migraElite(alim,arrayStructElite,k,numMigracoes,algoritmo);
                  
    end

    % roda Evolucao Diferencial de acordo com o tipo do algoritmo
    [populacao, fitness] = runAlgorithm(algoritmo, alim, populacao, fitness, k);
    
    % Elitismo. Guarda o melhor individuo
    arrayStructElite = preencheArrayStructElite(arrayStructElite,fitness,populacao,k);
    
    % verifica convergencia algoritmo
    [sucesso, arrayStructElite] = verificaSucesso(sistema,arrayStructElite,k,alim);

    if ( sucesso )
        return;
    end  
    
    % Atualiza geracao    
    k = k + 1; 

end
    
end

% verifica diversidade populacional
function baixaDiversidade = verificaDiversidadePopulacional(arrayStructElite,geracao)

fbest = arrayStructElite(1, geracao).FFxXGen;

fmed = arrayStructElite(1, geracao).FFxmedioXGen;

% OBS: o idg esta implentado conforme a sugestao do artigo (Vasconcelos, et alli, 2001) 
% gmg original fmed/fbest 
% caso a funcao fitness fosse positiva (isto é, inviduos mais bem 
% adaptados -> fitness maior), sugere-se utilizar idg = fbest/fmed,

idg = abs(fmed/fbest);

global paramAG;

if ( idg <= paramAG.limiteIDG)
    
    baixaDiversidade = true;

else
    
    baixaDiversidade = false;

end

% DEBUG
fbestDEBUG = 1/fbest;
fmedDEBUG = 1/fmed;
idgDEBUG = 1/idg;
limIDG = 1/paramAG.limiteIDG;

if (idgDEBUG > limIDG)
    
    debug = 0;
    
end

end
  
% verifica sucesso 
function [sucesso, arrayStructElite] = verificaSucesso(sistema,arrayStructElite,geracao,alim)

% TODO
% condicao de saida funcao
if (~strcmp(alim.Ftipo,'ieee'))

    sucesso = 0;
    return ;
end

% get elite da geracao
elite = arrayStructElite(1, geracao).FmelhorIndXGen; 

% get otimo
indOtimos = getIndividuoOtimo(sistema,alim);

% se ha mais de 1 individuo otimo
if (size(indOtimos,1)>1)

    %
    comparacao = ( indOtimos(1,:) == elite ) | ( indOtimos(2,:) == elite );
    
else
   
    % compara individuos
    comparacao = indOtimos == elite;

end

% preenche flag sucesso
if (all(comparacao))
    
    sucesso = true;

else
    
    sucesso = false;
    
end

end

% Elitismo. Guarda o melhor individuo
function arrayStructElite = preencheArrayStructElite(arrayStructElite,fitness,populacao,geracao)

% obtem indice do elite 
[eliteHx, indiceElite] = min(fitness);
individuoElite = populacao(indiceElite,:);

% vetor que guarda melhor individuo da geracao
arrayStructElite(geracao).FmelhorIndXGen =  individuoElite;

% Guarda valor da funcao para elite
arrayStructElite(geracao).FFxXGen = eliteHx; 

% Guarda media dos fitness
arrayStructElite(geracao).FFxmedioXGen = mean(fitness);

end

% cria array de structs elite 
function arrayStructElite = criaArrayStructElite(numGeracoes,alim)

%retorna o numero de variaveis e dominio(range) dado o nome da funcao
[rangeFunction, nVars] = rangeFuncao(alim.funcao,alim);

global param;

eliteStruct = struct;
eliteStruct = setfield(eliteStruct,'FmelhorIndXGen', []);
eliteStruct = setfield(eliteStruct,'FFxXGen', []);
eliteStruct = setfield(eliteStruct,'FFxmedioXGen', []);
eliteStruct.FmelhorIndXGen = zeros(nVars,1,param.dataType);
eliteStruct.FFxXGen = 0;
eliteStruct.FFxmedioXGen = 0;

for i=1:numGeracoes

    arrayStructElite(i) = eliteStruct;
    
end

end
