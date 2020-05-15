% obtem Chave Menor Perda por descida coordenada
% OBS: assinatura antiga
function chaveMenorPerda = obtemChaveMenorPerdaDescCoord( tieSwitch, alimOriginal, indArestasDoCiclo)

% % DEBUG
% if (tieSwitch==158)
%   DEBUG=0;  
% end

% transforma vetor de arestas do ciclo em uma estrutura alim. 
% esta funcao foi necessaria para reuso da funcao abaixo.  
alimCiclo = sparse2alim(indArestasDoCiclo,alimOriginal); 

% cria matriz sparsa do Ciclo
sparseCiclo = criaSparseMBranch(alimCiclo);

% % obtem chave de menor perda para o ciclo
chaveMenorPerda = obtemChaveMenorPerdaCaminhaRede(sparseCiclo, tieSwitch,alimOriginal);

end

% funcao de adaptacao, para reuso da funcao obtemChaveMenorPerdaCaminhaRede
% esta funcao cria, a partir do vetor das arestas que compoe um ciclo um
% objeto, o equivalente ao alimentador reduzido
function alim = sparse2alim(mapVerticesArestas,alimOriginal)

% cria alimentador vazio
alim = criaAlimVazio();

% init cont
cont = 1;

% para cada aresta do ciclo, obtem info 
for i=mapVerticesArestas'
    
    % adicionica
    alim.FmBranch(cont,:) = alimOriginal.FmBranch(i,:);

    % inc cont. 
    cont = cont + 1;
    
end

end

% obtem chave menor perda, caminhando p/ os 2 lados da resde 
function novaTieSwitch = obtemChaveMenorPerdaCaminhaRede(sparseCiclo,tieSwitch,alimOriginal)

% obtem nos 
[noDireito,noEsquerdo] = getNos(tieSwitch,alimOriginal);

% obtem fitness de referencia para analise do ciclo
ind = getIndividuoCorrente(alimOriginal);

% avalia individuo
fitRef = avaliaPopulacaoBuscaLocal(ind, alimOriginal);

% caminha em direcao ao Pai, enquanto fitness estiver diminuindo
novaTieSwitch = caminhaEnquantoFitnessDiminuir(noDireito,sparseCiclo,tieSwitch,fitRef,alimOriginal);

% senao encontrou chave menor perda, caminha no sentido do filho
if ( novaTieSwitch == tieSwitch )
    
    % caminha em direcao ao Pai, enquanto fitness estiver diminuindo
    novaTieSwitch = caminhaEnquantoFitnessDiminuir(noEsquerdo,sparseCiclo,tieSwitch,fitRef,alimOriginal);
    
end

end

% caminha em direcao ao Pai, enquanto fitness estiver diminuindo
function chaveTSmenorPerda = caminhaEnquantoFitnessDiminuir(noInicial,grafConectividade,tieSwitch,fitRef,alimOrig)

% init vars
lstNosVisitados =[];
chaveTSmenorPerda = tieSwitch; % caso nao encontre nenhuma TS melhor, retorna a TS antiga

% % DEBUG
% indOri = getIndividuoCorrente(alimRed);
% debug = avaliaPopulacao(indOri, alimRed);  

% cria novo individuo, fechando chave antiga e abrindo nova
% fecha TS antiga
alimOrig = fechaChaveTS(alimOrig,tieSwitch);   

% DO  caminha rede
[proxNo,lstNosVisitados] = caminhaRede(noInicial,lstNosVisitados,grafConectividade);
 
% enquanto fitness diminuir
while ( ~isempty(proxNo) )
      
    % obtem chave (indice aresta) por meio dos nos pais e filhos.
    novaTS = getChavePorNos(proxNo,noInicial,alimOrig);

    % abre nova chave TS 
    alimOrig = abreChaveTS(alimOrig,novaTS);
    
    % individuo corrente
    indBin = getIndividuoCorrente(alimOrig);
  
%     DEBUG 
    % verifica radialidade
%     radialBool = verificaRadialidadeBGL(alimOrig,indBin);

    % avalia individuo 
    [ novaFitness, alimOrig ] = avaliaPopulacaoBuscaLocal( indBin, alimOrig);
    
    % se novaFitness eh menor que fitness referencia, continua caminhando.
    if ( novaFitness < fitRef )
    
        % atualiza fitness de referencia
        fitRef = novaFitness;
        
        % atualiza chaveTSmenorPerda
        chaveTSmenorPerda = novaTS;
        
        % atualiza o noInicial armazenando o proxNo para poder atualizar o proxNo.
        noInicial = proxNo;
        
        % atualizada o alim, fechando TS antiga (antes de caminhar na rede)
        alimOrig = fechaChaveTS(alimOrig,novaTS);   

        % caminha rede
        [proxNo,lstNosVisitados] = caminhaRede(proxNo,lstNosVisitados,grafConectividade);

    else
                
        % break, retornando a chaveTSmenorPerda
        break;
        
    end
    
end

end

% caminha rede 
function [nosFilhos,lstNosVisitados] = caminhaRede(no,lstNosVisitados,grafoSparse)

% init vars
nosFilhos = [];

% so expande nos filhos se nao visitou o pai ainda
if ( ~ismember(no,lstNosVisitados) )

    % TODO verificar se funciona para os 2 lados. OPCAO usar getNosPorNos!!
    % lista de nos filhos
    nosFilhos = find(grafoSparse(no,:) == 1);

    % adiciona no atual como visitado
    lstNosVisitados = [no,lstNosVisitados];
    
    % verifica se algum dos nos filhos ja foi visitado
    nosFilhosVisitados = ismember(nosFilhos,lstNosVisitados);
    
    % remove nos ja visitados
    nosFilhos = nosFilhos(~nosFilhosVisitados);
   
end

% FIX ME rede1
% 2018 corrige erro em rede1 
% verifica tamanho chave 
if ( length(nosFilhos) > 1 )

    % mensagem de erro
    disp('Erro tamanho chave!')
    
    % 
    nosFilhos = nosFilhos(1);
    
end

end

% abre chave TS 
function alim = abreChaveTS(alim,chave)

% verifica tamanho chave 
if ( length(chave) > 1 )

    % mensagem de erro
    disp('Erro tamanho chave!')
    
end

% obtem individuo corrente
indBin = getIndividuoCorrente(alim);

% desliga chave interconexao (retira laco)
indBin(chave) = 0;

% seta individuo corrente 
alim = setaBranchsAtivos(alim,indBin);   

end

% fecha chave TS 
function alim = fechaChaveTS(alim,chave)

% obtem individuo corrente
indBin = getIndividuoCorrente(alim);

% desliga chave interconexao (retira laco)
indBin(chave) = 1;

% seta individuo corrente 
alim = setaBranchsAtivos(alim,indBin);   

end