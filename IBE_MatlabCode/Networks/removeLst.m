% remove elem da lista  
function lst = removeLst(elem,lst)

% OBS: OLD CODE 
% %indices
% ind = find(lst == elem );
% 
% %remove
lst = lst(lst~=elem);

end


