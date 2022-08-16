% mutacao DE Elite original
function e = mutacaoDEOriginalCodTieSwElite(radialBool, populacao)

% Np = tamanho populacao
Np = size(populacao,1); 

% condicao de saida
if (Np==3)||(Np==2)||(Np==1)
    e=[];
    return;
end

% get elite
elite = populacao(1,:);

% atualiza indices for 
indNaoRadiais = find(radialBool==false);

global Fm; % fator de escala mutacao
global Crm; % fator de escala mutacao

% escolhe indice de individuo que ainda nao eh radial 
for i= indNaoRadiais' % i=1 : Np

    % escolhe R1 (individuo 1) aleatoriamente
    r1 = i;
    while ((r1 == i))
        r1 = round ( 1 + (Np-1)*rand() );
    end

    % escolhe R2 (individuo 2) aleatoriamente
    r2 = i;
    while ((r2 == i) || (r2 == r1))
        r2 = round ( 1 + (Np-1)*rand() );
    end

% % DE original
   
    % numero de tieswitchs 
    D = size(populacao,2);

    % seleciona inteiro p/  que sera cruzada]
    % gera numero aleatorio entre 1 e D
    jrand = 1 + (D-1)*rand();
    jrand = round(jrand);
       
%   cruzamento binomial (ou uniforme)
    for j=1:D 
         
       if ( ( rand() <= Crm ) || ( j == jrand) ) 
            
            e(i,j) = elite(1,j) + Fm*( populacao(r1,j)-populacao(r2,j));
            
        else
            
            e(i,j) = elite(1,j); 
        
       end
        
    end
    
end

end
