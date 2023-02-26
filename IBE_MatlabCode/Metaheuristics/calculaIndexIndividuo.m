% calcula Index Individuo
function index = calculaIndexIndividuo(individuo)

% acha indice de chaves fechadas
[a, individuo] = find(individuo==0);

% 2023 order
individuo = sort(individuo);

% 
if (~isempty(individuo))

    % transforma vetor de indices em string e poda espacos
    index = strrep(num2str(individuo), ' ', '');

else % se indice eh vazio (por exemplo, individuo todo fechado), atribui indice = 
     
    index ='0';    
    
end

end