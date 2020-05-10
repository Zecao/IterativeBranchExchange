# 2020Dijkstra
Repository for the article "Applying the Dijkstra’s Algorithm in Distribution Networks Reconfiguration for Energy Losses Optimization".
This repository is subdivided in the following subdirectories: 

## 1. Suplementary material for the article
[**"PDFfiles"**](https://github.com/Zecao/2020Dijkstra/tree/master/pdfFiles)
Directory with the article and suplementary material pdfs files.

## 2. Initial population of evolutionary metaheuristics for the Distribution Network Reconfiguration problem
[**"MSTInitialPopulation"**](https://github.com/Zecao/2020Dijkstra/tree/master/MSTInitalPopulation)
Directory with the initial populations used in the article for the Distribution Network Reconfiguration - DNR problem. The files are in Matlab (.mat) format and uses the binary codification, i.e. they represent the normally closed (NC) switches by 1s and the normally opened (NO) switches by 0s for the following test cases:  
* (Zhu, 2002) 33 buses system
* (Huang, 2002) 69 (or 70) buses system
* (Su & Lee, 2003) 84 buses system
* (Guimarães & Castro, 2005) 136 buses system
* (Ramirez-rosado & Bernal-Agustín, 1998) 417 (or 415) buses system. 

## 3. Article Matlab code
[**"DijkstraBEMatlabCode"**](https://github.com/Zecao/2020Dijkstra/tree/master/DijkstraBE_MatlabCode)
Directory with the Matlab code for replication of all the results from article. 
The main script is the file "" 
The code is mainly with portuguese comments, so I apologize for the non-portugues speaking.   
 
## 4. Article OpenDSS customization code
[**"DijkstraBranchExchangeOpenDSS"**](https://github.com/Zecao/2020Dijkstra/tree/master/DijkstraBE_OpenDSSCustomization)
Directory with the Visual Studio C# OpenDSS customization project. 
The project also uses 2 dll files: EEPlus.dll (that allows Excel files to be read in the C#) and Auxiliares.dll (the co author Daniel Rocha routines) already included in the project. Essencially its a newer version from the project https://github.com/Zecao/ExecutorOpenDssBr  

Ezequiel C. Pereira
