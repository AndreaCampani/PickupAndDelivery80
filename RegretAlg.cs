using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//modifica
namespace Pickup_and_Delivery_E80
{
	class AlgRegret
	{
		private int size;
		private double[][] MatCopy;
		private List<int> row;
		private List<int> column;
		private List<int> CallAdvance;
		private int P_count;
		private int D_count;
		private int St_count;
		private int H_count;
		private int F_count;
		private Problem f;
		private Solution sol;
		public string algorithm_name;//ciao
		public AlgRegret(Problem p)
		{
			this.f = p;
			this.P_count = p.pickup_count;
			this.D_count = p.delivery_count;
			this.St_count = p.stPoint_count;
			this.H_count = p.home_count;
			this.F_count = p.fillup_count;
			this.size = p.pickup_count + p.delivery_count + p.stPoint_count + p.home_count + p.fillup_count;//dimensione matrice (size) 
			this.MatCopy = new double[size][];//matrice copia di times
			this.row = new List<int>();
			this.column = new List<int>();
			this.algorithm_name = "Regret";
			this.sol = new Solution(p,algorithm_name);
			for (int i = 0; i < size; i++)
			{
				this.MatCopy[i] = new double[size];
			}
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					this.MatCopy[i][j] = p.GetTime(i, j);
				}
			}
		}
		public void Routing()
		{
			int ind;//da eliminare
			int ind1;//da eliminare
			int ind3;
			int[] Memory = new int[2];
			double[] Memory1 = new double[2];
			double[] AgvsTime = new double[St_count];
			int[] AgvsPosition = new int[St_count];
			for (int i = 0; i < P_count; i++)
			{ 
			    column.Add(i);
			}
			for(int i=P_count+D_count;i< P_count + D_count+St_count;i++)
			{
				row.Add(i);
				sol.AssignNode(sol.GetNode(i), 0, 0, sol.GetNode(i).GetServiceTime(), 0, -1, -1, f.GetVehicle(i - 2 * D_count).GetID(), false);
			}
			while (P_count!=0)
			{
				while (St_count != 0)
				{
					Memory = assignment(row, column, MatCopy,0,f.GetNode());
					//sol.AssignMission(sol.GetMissionFromPickPos(Memory[1]),0,//id st,-1,//id veicolo ,true);//assegna missione al veicolo
					Console.WriteLine(" " + Memory[0] + " " + Memory[1]);
					ind = Memory[0] - 2 * D_count+1;
					ind1 = Memory[1]+1;
					Console.WriteLine("St"+ind+" to P"+ind1+"\r");
					row.Remove(Memory[0]);
					column.Remove(Memory[1]);
					Console.WriteLine("posizioni:");
					AgvsTime[Memory[0]-2*D_count] = MatCopy[Memory[0]][Memory[1]]+MatCopy[Memory[1]][Memory[1]+D_count];
					AgvsPosition[Memory[0] - 2 * D_count] =Memory[1]+D_count;
					Console.WriteLine("Time:"+AgvsTime[Memory[0] - 2 * D_count]+" "+"position:"+ AgvsPosition[Memory[0] - 2 * D_count]);
					ind3= Memory[0] - 2 * D_count + 1;
					Console.WriteLine("Vehicle"+ind3+" to D"+ind1+"\r");
					St_count--;
					P_count--;
				}
				if(P_count>0)
				{
					row.Clear();
					Memory1 = SearchMin(AgvsTime);
					row.Add(AgvsPosition[(int)Memory1[1]]);
					Memory = assignment(row, column, MatCopy,Memory1[0],f.GetNode());
					Console.WriteLine(" " + Memory[0] + " " + Memory[1]);
					ind = (int)Memory1[1] + 1;//posizione per stampa
					ind1 = AgvsPosition[ind-1] + 1-D_count;//come sopra
					ind3 = Memory[1] + 1;
					AgvsTime[(int)Memory1[1]] = AgvsTime[(int)Memory1[1]]+ MatCopy[Memory[0]][Memory[1]] + MatCopy[Memory[1]][Memory[1] + D_count];//fix indice oltre i limiti matrice
					AgvsPosition[(int)Memory1[1]] = Memory[1] + D_count;
					Console.WriteLine("Vehicle" + ind + " in posizione D"+ind1+"to P"+ind3+"\r");
					ind1 = ind3 ;
					Console.WriteLine("Vehicle" + ind + " in posizione P" + ind3 + "to D" + ind1 + "\r");
					//sol.AssignMission()
					row.Clear();
					column.Remove(Memory[1]);
					P_count--;
				}
			}
		}
		public int[] assignment(List<int> riga, List<int> colonna,double[][] Matrix,double arrival, List<Node> g)//aggiungere lista nodi per la tw
		{
			int size1 = riga.Count;
			int size2 = colonna.Count;
			double[] SottoMat = new double[size2];
			double delta = 0;
			double delta1;
			int[] Position = new int[2];
			for (int i = 0; i < size1; i++)
			{
				for (int j = 0; j < size2; j++)
				{
					SottoMat[j] =Math.Max(arrival+Matrix[riga[i]][colonna[j]],g[colonna[j]].GetEarliestTime());
				}
				delta1 = CalcolaDelta(SottoMat);
				if (delta1 > delta)
				{
					Position[0] = riga[i];
					delta = delta1;
					Position[1] = colonna[(int) SearchMin(SottoMat)[1]];
				}
			}
			return Position;
		}

		public double CalcolaDelta(double[] t)
		{
			int size;
			size = t.Length;
			double[] b = new double[size];
			t.CopyTo(b, 0);
			double delta;
			double delta1;
			double[] min = new double[2];
			min = SearchMin(b);
			delta = min[0];
			b[(int)min[1]] = int.MaxValue;
			delta1 = SearchMin(b)[0];
			return Math.Abs(delta - delta1);
		}
		public double[] SearchMin(double[] array)//da in output un array di dim=2, con il min in 0 e la sua posizione nell'array in 1
		{
			double[] risultato = new double[2];
			double min;//minimo
			min = array[0];
			int position = 0;//posizione minimo
			for (int i = 1; i < array.Length; i++)
			{
				if (array[i] < min) { min = array[i]; position = i; }
			}
			risultato[0] = min;
			risultato[1] = position;
			return risultato;
		}
		/*
		public int[] trovamax(int[] array)//da in output un array di dim=2, con il max in 0 e la sua posizione nell'array in 1
		{
			int[] risultato = new int[2];
			int max = array[0];//massimo
			int position = 0;//posizione di massimo
			for (int i = 1; i < array.Length; i++)//ricerca massimo
			{
				if (array[i] > max) { max = array[i]; position = i; }
			}
			risultato[0] = max;
			risultato[1] = position;
			return risultato;
		}
		public int[][] regret(int[][] costi)
		{
			int v1;//v1,v2 saranno le cordinate della posizione da assegnare
			int v2;
			int righe_count = costi.Length;
			int colonne_count = costi[0].Length;
			int[][] assegnamenti = new int[1][];//array di output con le cordinate 
			int[] memoria = new int[colonne_count];//una copia di "costi" per non modificare la matrice dei costi
			int[] delta = new int[righe_count];//valore di criticità
			int[] memoriamin = new int[2];//tengo memorizzati i min/max
		    int[] memoriamax = new int[2];
			for (int i = 0; i < righe_count; i++)
			{
				memoriamin = trovamin(costi[i]);
				v1 = memoriamin[0];
				memoriamax = trovamax(costi[i]);
				costi[i].CopyTo(memoria, 0);
				memoria[memoriamin[1]] = memoria[memoriamin[1]] + memoriamax[0];
				v2 = trovamin(memoria)[0];
				delta[i] = Math.Abs(v1 - v2);
			}
			v1 = trovamax(delta)[1];//il max del delta
			v2 = trovamin(costi[v1])[1];
			assegnamenti[0] = new int[2] { v1, v2 };
			return assegnamenti;
		}
		
		public int[][] copia(int[][] mat)
		{
			int righe_count = mat.Length;
			int colonne_count = mat[0].Length;
			int[][] MatCopia = new int[righe_count][];
			for (int i = 0; i < righe_count; i++)
			{
				MatCopia[i] = new int[colonne_count];
			}
			for (int i = 0; i < righe_count; i++)
			{
				for (int j = 0; j < colonne_count; j++)
				{
					MatCopia[i][j] = mat[i][j];
				}
			}
			return MatCopia;
		}
		*/
	}
}
