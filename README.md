# 2020Dijkstra
Repository for the article "Applying the Dijkstra’s Algorithm in Distribution Networks Reconfiguration for Energy Losses Optimization".
This repository is subdivided in the following subdirectories: 

## 1. Suplementary material for the article
[**directory "SupMat"**](https://github.com/Zecao/2020Dijkstra/tree/master/SupMat)

## 2. Initial Population for Evolutionary Metaheuristics
[**directory "MSTInitialPopulation"**](https://github.com/Zecao/2020Dijkstra/tree/master/MSTInitalPopulation)
Directory with the initial populations for the Distribution Network Reconfiguration - DNR used to achieve the results in the article.
The files are in Matlab (.mat) format and uses the binary codification, i.e. they represent the normally closed (NC) switches by 1s and the normally opened (NO) switches by 0s for the following test cases:  
* (Zhu, 2002) 33 buses system
* (Huang, 2002) 69 (or 70) buses system
* (Su & Lee, 2003) 84 buses system
* (Guimarães & Castro, 2005) 136 buses system
* (Ramirez-rosado & Bernal-Agustín, 1998) 417 (or 415) buses system. 

## 3. Article Matlab code
[**directory "DijkstraMatlabCode"**](https://github.com/Zecao/2020Dijkstra/tree/master/DijkstraMatlabCode)
Directory with the Matlab code for. The main script is the file. The code is mainly with portuguese comments, so I apologize for the non-portugues speaking.   
 
## 4. OpenDSS Customization 
[**directory "DijkstraBranchExchangeOpenDSS"**](https://github.com/Zecao/2020Dijkstra/tree/master/DijkstraBranchExchangeOpenDSS)
Directory with the C# OpenDSS customization project. I think that one must be familiar with GeoPerdas/BDGD openDSS format files. For more information see my project 

Ezequiel C. Pereira
