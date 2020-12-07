#define ENGINE
#if ENGINE
using OpenDSSengine;
#else
using dss_sharp;
#endif

using System.Collections.Generic;
using System.Linq;
using QuickGraph;
using QuickGraph.Algorithms;
using ExecutorOpenDSS.Reconfigurador;

namespace ExecutorOpenDSS.Classes_Principais
{
    class FeederGraph
    {
        //private string _BusLevels = "_Bus_Levels.xlsx";
        private readonly string _Cols = "_Inc_Matrix_Cols.csv";
        private readonly string _IncMatrix = "_Inc_Matrix.csv";
        private readonly string _prefix = "alim";

        // classe dicionario NomeBranchs X Indices
        public DicNomeBranchs _dicNomeBranchs = new DicNomeBranchs();

        // TODO encapsular _dicNomePACs X Indices
        // MAP nomeVertices -> indice
        public Dictionary<string, string> _mapNomeVertice2Indice;        

        // MAP vertices(fonteXcarga) -> Aresta
        Dictionary<string, string> _mapIndVertices2Aresta;    

        // matriz incidencia Aresta - lista Vertices
        Dictionary<string, List<string>> _matrizIncidencia;       

        // Define some weights to the edges
        Dictionary<Edge<string>, double> _edgeCost;
        
        // TODO
        //Func<Edge<string>, double> edgeCost = e => 1; // constant cost

        // Grafo
        UndirectedGraph<string, Edge<string>> _quickGraph;

        // lista nome chaves NF
        public List<string> _lstNomeChavesNFcam1;

        private GeneralParameters _paramGerais;
        private MainWindow _janela;
        private Text _DSSText;

        public FeederGraph(GeneralParameters par, MainWindow jan, Text DSSText)
        {
            _paramGerais = par;
            _janela = jan;

            _DSSText = DSSText;

            // get matriz incidencia da API OpenDSS
            GetMatrizIncidencia();

            // carrega arquivos CSV da matriz incidencia
            CarregaMapsGrafoAlim();
        }

        // obtem nome do vertice
        public string GetNomeVertice(string vertice)
        {
            if (_mapNomeVertice2Indice.ContainsKey(vertice))
                return _mapNomeVertice2Indice[vertice];
            else
                return null;
        }

        //
        private void GetMatrizIncidencia()
        {
            // Calcula matriz de incidencia ordenada
            _DSSText.Command = "CalcIncMatrix_O";

            // Matriz incidencia
            _DSSText.Command = "Export IncMatrix";

            //!!Exports the name of the cols(buses)
            _DSSText.Command = "Export IncMatrixRow";

            //!!Exports the name of the rows(link branches)
            _DSSText.Command = "Export IncMatrixCols";

            // OBS BusLevels nao utilizado
            //dSSText.Command = "Export BusLevels";
        }

        // 
        public void CarregaMapsGrafoAlim()
        {
            // preenche map Nome Branchs 
            _dicNomeBranchs.PreencheMapNomeBranchs( _paramGerais.GetNomeAlimAtual() );

            // preenche map Nome Vertices
            PreencheMapNomeVertices();

            // carrega matriz incidencia
            CarregaCSVMatrizInc();

            // cria objeto QuickGraph
            CriaObjQuickGraph();
        }

        // Cria objeto QuickGraph
        private void CriaObjQuickGraph()
        {
            //QuickGraph            
            _quickGraph = new UndirectedGraph<string, Edge<string>>();

            // map de custos
            _edgeCost = new Dictionary<Edge<string>, double>();

            // para todos os elementos da matriz de incidencia
            foreach (KeyValuePair<string, List<string>> kvp in _matrizIncidencia)
            {  
                // aresta
                string sAresta1 = kvp.Key;

                // vertices
                string vertice1 = (kvp.Value)[0];
                string vertice2 = (kvp.Value)[1];

                //cria aresta 
                Edge<string> aresta = new Edge<string>(vertice1, vertice2);

                // Adiciona vertices e arestas 
                _quickGraph.AddVerticesAndEdge(aresta);

                // Custo de todas as arestas eh 1
                _edgeCost.Add(aresta, 1);
            }
    
        }

        // Carrega arquivo matriz incidencia 
        private void CarregaCSVMatrizInc()
        {
            //_Inc_Matrix.csv"
            string nomeArqVertices = _prefix + _paramGerais.GetNomeAlimAtual() + _IncMatrix;

            // Le uma coluna
            List<string[]> matrizInc = XLSXFile.LeCSV(nomeArqVertices);

            //
            _matrizIncidencia = new Dictionary<string, List<string> >();
            _mapIndVertices2Aresta = new Dictionary<string, string> ();
            
            //OBS: comeca a contar de 1, porque 0 eh o cabecalho (nome da coluna) exportado pelo OpenDSS
            for (int i = 1; i < matrizInc.Count(); i++)
            {
                // arestas
                string sAresta1 = matrizInc[i][0];

                // vertices
                string vertice1 = matrizInc[i][1];

                // adiciona o 1o. vertice caso nao contenha nada 
                if (!_matrizIncidencia.ContainsKey(sAresta1))
                {

                    // adiciona vertice na matriz de incidencia
                    _matrizIncidencia.Add(sAresta1, new List<string>() { vertice1 });                  

                }
                else
                {
                    // pega lista atual 
                    List<string> lstVertices = _matrizIncidencia[sAresta1];

                    // remove aresta
                    _matrizIncidencia.Remove(sAresta1);

                    // acrescenta novo vertice 
                    lstVertices.Add(vertice1);

                    // coloca nova list ano grafo
                    _matrizIncidencia.Add(sAresta1, lstVertices);


                    // NOVO codigo #######
                    // preenche map listaVertices -> Aresta
                    string strLstVertices = lstVertices[0] + "." + lstVertices[1];

                    // adiciona aresta no MAP 
                    if (!_mapIndVertices2Aresta.ContainsKey(strLstVertices))
                    {
                        _mapIndVertices2Aresta.Add(strLstVertices, sAresta1);
                    }
                }
            }
        }

        // roda menor caminho em uma chave NA
        internal bool MenorCaminho(Switch chaveNAinicial)
        {
            // traduz chave
            string no1 = chaveNAinicial.bus1;
            string no1trad = GetNomeVertice(no1);
            string no2 = chaveNAinicial.bus2;
            string no2trad = GetNomeVertice(no2);

            bool ret = false;

            // TODO verificar
            // se chave NA possui os 2 vertices 
            if ( ( no1trad != null)&&( no2trad != null) )
            {
                ret = MenorCaminhoDijsktra(no1trad, no2trad);
            }
                        
            // 
            return ret;
        }

        // roda menor caminho em uma chave NA
        internal bool MenorCaminhoDijsktra(string no1, string no2)
        {
            /* //DEBUG
            var vertiNo1 = _edgeCost.Where(x => x.Key.Source == no1 || x.Key.Target == no1).ToList();
             */

            // compute shortest paths
            TryFunc<string, IEnumerable<Edge<string>>> tryGetPaths = _quickGraph.ShortestPathsDijkstra(x => _edgeCost[x], no1);

            // caminho ate cabeca alim (ROOT) lado1
            IEnumerable<Edge<string>> path1;

            // obtem lstChavesNFs do no2 ate o caminho do no1  
            if (tryGetPaths(no2, out path1))
            {
                _lstNomeChavesNFcam1 = FiltraArestasComChaves(path1);               
            }

            // DEBUG Verifica se segundo caminho eh igual ao primeiro
            /*
            TryFunc<string, IEnumerable<Edge<string>>> tryGetPaths2 = _quickGraph.ShortestPathsDijkstra(x => _edgeCost[x], no2);

            // caminho ate cabeca alim (ROOT) lado1
            IEnumerable<Edge<string>> path2;

            // obtem lstChavesNFs caminho 2  
            if (tryGetPaths2(no1, out path2))
            {
                _lstNomeChavesNFcam2 = filtraArestasComChaves(path2);
            }
            */
            return true;
        }

        // filtra arestas com chaves 
        private List<string> FiltraArestasComChaves(IEnumerable<Edge<string>> caminho)
        {
            // variavel tmp
            List<string> lstNomeChavesNFcaminho = new List<string>(); 

            // para cada aresta do caminho
            foreach (Edge<string> aresta in caminho)
            {
                string indiceVertFonte = aresta.Source;
                string indiceVertCarga = aresta.Target;
                
                string lstVertices1 = indiceVertFonte + "." + indiceVertCarga;
                string lstVertices2 = indiceVertCarga + "." + indiceVertFonte;

                string indiceAresta = null;

                // se map contem 
                if (_mapIndVertices2Aresta.ContainsKey(lstVertices1) || _mapIndVertices2Aresta.ContainsKey(lstVertices2))
                {
                    indiceAresta = _mapIndVertices2Aresta[lstVertices1];
                
                    // verifica se pode ser manobrada
                    string nomeChave = _dicNomeBranchs.GetNomeBranchByIndice(indiceAresta);

                    // verifica se eh chave
                    bool teste = Switch.IsChave(_DSSText, nomeChave);

                    // armazena chave NF na lista 
                    if (teste)
                    {
                        // remove o "line."
                        nomeChave = nomeChave.Remove(0, 5);

                        lstNomeChavesNFcaminho.Add(nomeChave);
                    }
                }

                if (_mapIndVertices2Aresta.ContainsKey(lstVertices2)) 
                {
                    _janela.ExibeMsgDisplayMW("detectada inversao de lista de Vertices");
                }

            }
            return lstNomeChavesNFcaminho;
        }

        // preenche map com nome dos vertices
        private void PreencheMapNomeVertices()
        {
            //
            string nomeArqVertices = _prefix + _paramGerais.GetNomeAlimAtual() + _Cols;

            // Le uma coluna
            List<string> nomeVertices = XLSXFile.Le1ColunaCSV(nomeArqVertices);

            //verificar se arquivo existe
            _mapNomeVertice2Indice = new Dictionary<string, string>();

            // OBS: comeca a contar de 1, porque 0 eh o cabecalho (nome da coluna) exportado pelo OpenDSS
            for (int i = 1; i < nomeVertices.Count(); i++)
            {
                // OBS: subtraio 1 de i, uma vez que na matriz exportada pelo openDSS, o primeiro no eh "0" 
                _mapNomeVertice2Indice.Add(nomeVertices[i], (i-1).ToString() );
            }
        }
    }
}

/*
 * https://github.com/YaccConstructor/QuickGraph/wiki/Dijkstra-Shortest-Distance-Example
Dijkstra Shortest Path Distance Example
Dijkstra extension methods
The AlgorithmExtensions class contains several helper methods to execute the algorithm on a given graph.

using QuickGraph;
using QuickGraph.Algorithms;

IVertexAndEdgeListGraph<TVertex, TEdge> graph = ...;
Func<TEdge, double> edgeCost = e => 1; // constant cost
TVertex root = ...;

// compute shortest paths
TryFunc<TVertex, TEdge> tryGetPaths = graph.ShortestPathDijkstra(edgeCost, root);

// query path for given vertices
TVertex target = ...;
IEnumerable<TEdge> path;
if (tryGetPaths(target, out path))
    foreach(var edge in path)
        Console.WriteLine(edge);

    Advanced use
This example sets up a Dijkstra shortest path algorithm and computes the distance of the vertices in the graph.

Func<TEdge, double> edgeCost = e => 1; // constant cost
// We want to use Dijkstra on this graph
var dijkstra = new DijkstraShortestPathAlgorithm<TEdge, TEdge>(graph, edgeCost);
Using a predecessor recorder to build the shortest path tree
Algorithms raise a number of events that observes can leverage to build solutions.For example, attaching a predecessor recorder to the Dijkstra algorithm will let us build a predecessor tree.This tree is later used to build shortest paths.

// Attach a Vertex Predecessor Recorder Observer to give us the paths
var predecessors = new VertexPredecessorRecorderObserver<TVertex, TEdge>();
using (predecessors.Attach(dijkstra))
    // Run the algorithm with A set to be the source
    dijkstra.Compute("A");
The predecessors instance now contains a dictionary of distance from each vertex to the source:

foreach (var v in graph.Vertices) {
    double distance = 0;
TVertex vertex = v;
TEdge predecessor;
     while (predecessors.VertexPredecessors.TryGetValue(vertex, out predecessor)) {
          distance += edgeCost[predecessor] (predecessor);
          vertex = predecessor.Source;
     }
     Console.WriteLine("A -> {0}: {1}", v, distance);
}
Using a dictionary for edge costs
Because the algorithm take a delegate as the edge cost, one cannot simply pass a dictionary. QuickGraph provides a helper method, GetIndexer, to make the conversion:

Dictionary<TEdge, double> edgeCostDictionary = ...
Func<TEdge, double> edgeCost = AlgorithmExtensions.GetIndexer(edgeCostDictionary);
...

 */

/*

AdjacencyGraph<string, Edge<string>> graph = new AdjacencyGraph<string, Edge<string>>(true);

// Add some vertices to the graph
graph.AddVertex("A");
graph.AddVertex("B");
graph.AddVertex("C");
graph.AddVertex("D");
graph.AddVertex("E");
graph.AddVertex("F");
graph.AddVertex("G");
graph.AddVertex("H");
graph.AddVertex("I");
graph.AddVertex("J");

// Create the edges
Edge<string> a_b = new Edge<string>("A", "B");
Edge<string> a_d = new Edge<string>("A", "D");
Edge<string> b_a = new Edge<string>("B", "A");
Edge<string> b_c = new Edge<string>("B", "C");
Edge<string> b_e = new Edge<string>("B", "E");
Edge<string> c_b = new Edge<string>("C", "B");
Edge<string> c_f = new Edge<string>("C", "F");
Edge<string> c_j = new Edge<string>("C", "J");
Edge<string> d_e = new Edge<string>("D", "E");
Edge<string> d_g = new Edge<string>("D", "G");
Edge<string> e_d = new Edge<string>("E", "D");
Edge<string> e_f = new Edge<string>("E", "F");
Edge<string> e_h = new Edge<string>("E", "H");
Edge<string> f_i = new Edge<string>("F", "I");
Edge<string> f_j = new Edge<string>("F", "J");
Edge<string> g_d = new Edge<string>("G", "D");
Edge<string> g_h = new Edge<string>("G", "H");
Edge<string> h_g = new Edge<string>("H", "G");
Edge<string> h_i = new Edge<string>("H", "I");
Edge<string> i_f = new Edge<string>("I", "F");
Edge<string> i_j = new Edge<string>("I", "J");
Edge<string> i_h = new Edge<string>("I", "H");
Edge<string> j_f = new Edge<string>("J", "F");

// Add the edges
graph.AddEdge(a_b);
graph.AddEdge(a_d);
graph.AddEdge(b_a);
graph.AddEdge(b_c);
graph.AddEdge(b_e);
graph.AddEdge(c_b);
graph.AddEdge(c_f);
graph.AddEdge(c_j);
graph.AddEdge(d_e);
graph.AddEdge(d_g);
graph.AddEdge(e_d);
graph.AddEdge(e_f);
graph.AddEdge(e_h);
graph.AddEdge(f_i);
graph.AddEdge(f_j);
graph.AddEdge(g_d);
graph.AddEdge(g_h);
graph.AddEdge(h_g);
graph.AddEdge(h_i);
graph.AddEdge(i_f);
graph.AddEdge(i_h);
graph.AddEdge(i_j);
graph.AddEdge(j_f);

// Define some weights to the edges
Dictionary<Edge<string>, double> edgeCost = new Dictionary<Edge<string>, double>(graph.EdgeCount);
edgeCost.Add(a_b, 4);
edgeCost.Add(a_d, 1);
edgeCost.Add(b_a, 74);
edgeCost.Add(b_c, 2);
edgeCost.Add(b_e, 12);
edgeCost.Add(c_b, 12);
edgeCost.Add(c_f, 74);
edgeCost.Add(c_j, 12);
edgeCost.Add(d_e, 32);
edgeCost.Add(d_g, 22);
edgeCost.Add(e_d, 66);
edgeCost.Add(e_f, 76);
edgeCost.Add(e_h, 33);
edgeCost.Add(f_i, 11);
edgeCost.Add(f_j, 21);
edgeCost.Add(g_d, 12);
edgeCost.Add(g_h, 10);
edgeCost.Add(h_g, 2);
edgeCost.Add(h_i, 72);
edgeCost.Add(i_f, 31);
edgeCost.Add(i_h, 18);
edgeCost.Add(i_j, 7);
edgeCost.Add(j_f, 8);

// We want to use Dijkstra on this graph
DijkstraShortestPathAlgorithm<string, Edge<string>> dijkstra = new DijkstraShortestPathAlgorithm<string, Edge<string>>(graph, edgeCost);

// attach a distance observer to give us the shortest path distances
VertexDistanceRecorderObserver<string, Edge<string>> distObserver = new VertexDistanceRecorderObserver<string, Edge<string>>();
distObserver.Attach(dijkstra);

// Attach a Vertex Predecessor Recorder Observer to give us the paths
VertexPredecessorRecorderObserver<string, Edge<string>> predecessorObserver = new VertexPredecessorRecorderObserver<string, Edge<string>>();
predecessorObserver.Attach(dijkstra);

// Run the algorithm with A set to be the source
dijkstra.Compute("A");

foreach (KeyValuePair<string, int> kvp in distObserver.Distances)
Console.WriteLine("Distance from root to node {0} is {1}", kvp.Key, kvp.Value);

foreach(KeyValuePair<string, Edge<string>> kvp in predecessorObserver.VertexPredecessors)
Console.WriteLine("If you want to get to {0} you have to enter through the in edge {1}", kvp.Key, kvp.Value );

// Remember to detach the observers
distObserver.Detach(dijkstra);
predecessorObserver.Detach(dijkstra);

*/
