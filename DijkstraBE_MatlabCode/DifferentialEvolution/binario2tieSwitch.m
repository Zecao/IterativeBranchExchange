% transforma populacao binaria p/ tie switch
function popTS = binario2tieSwitch(popBin,alim)

% 2019 NEW CODE
% transforma individuo TS para individuo TS que mapeiam somente arestas que podem ser manobradas,
% isto, eh arestas que fazem parte do conjunto de ciclos do alimentador.
global paramAG
if (paramAG.indTSred==1)
    indicesManobraveis = cast(alim.FmChavesIEEE(:,5),'logical');
    popBin = popBin(:,indicesManobraveis');
end

% versao 1.0
popTS = binario2tieSwitchPvt(popBin);

% versao 2.0 (alim como parametro)
if ( ~isempty(alim) )
    
    % traduz chaves cemig
    if ( strcmp(alim.Ftipo,'cemig') )

        % TODO; reescrever funcao que elimine esta traducao trabalhando sobre 
        % os branchs manobraveis
        % traduz indices Cemig
        popTS = indicesMBranch2ChavesIEEECemig(popTS,alim);
        
    end
    
end

end

function newPop = binario2tieSwitchPvt(populacao)

newPop = [];

%
for i=1:size(populacao,1)
    
    individuo = populacao(i,:);

    % indices chaves fechadas
    indFechado = find(individuo==0);
    
    newPop(i,:) = indFechado;
    
end

end

% transforma individuo TS para individuo TS que mapeiam somente arestas que podem ser manobradas,
% isto, eh arestas que fazem parte do conjunto de ciclos do alimentador.
function newPop = tieSwitch2TSReduzido(newPop,alim)

switch alim.Fnome 
    
    case 'alim\Sistema_Zhu2002_3fontes.txt';

    case 'alim\Sistema_Zhu2002.txt';        
            
    case 'alim\Sistema_Huang2002.txt'        

    case 'alim\Sistema_Brasileiro2008.txt';
        
        DePAra = Rede4DePara();
        

     
    case 'alim\Sistema_TPC2003.txt';           

    case 'alim\Sistema_415.txt';

    otherwise
        
end

end


function DePara = Rede4DePara()

DePara = [
1	1
2	2
3	3
4	4
5	5
6	6
7	7
8	8
9	9
10	10
13	11
15	12
17	13
18	14
19	15
20	16
22	17
24	18
25	19
26	20
27	21
28	22
31	23
35	24
38	25
39	26
40	27
42	28
43	29
45	30
46	31
47	32
48	33
49	34
50	35
51	36
52	37
53	38
54	39
55	40
62	41
63	42
64	43
65	44
66	45
67	46
68	47
70	48
73	49
75	50
76	51
77	52
78	53
79	54
80	55
81	56
83	57
84	58
85	59
86	60
88	61
89	62
90	63
91	64
92	65
93	66
94	67
95	68
96	69
97	70
98	71
99	72
100	73
101	74
103	75
104	76
105	77
106	78
107	79
110	80
118	81
119	82
120	83
121	84
122	85
123	86
125	87
126	88
127	89
128	90
129	91
130	92
131	93
132	94
133	95
134	96
135	97
136	98
137	99
138	100
139	101
140	102
141	103
142	104
143	105
144	106
145	107
146	108
147	109
148	110
149	111
150	112
151	113
152	114
153	115
154	116
155	117
156	118];

end



