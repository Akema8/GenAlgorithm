using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Pract
{
    class Gene
    {
        public int stock; //склад
        public int order; //заказ
        public int size; //доставляемая величина от данного склада данному заказу
        public Gene(int x, int y, int z)
        {
            stock = x;
            order = y;
            size = z;
        }
    }

    class Osob
    {
        public List<Gene> osob = new List<Gene>();
        public List<int> stokIn = new List<int>(); //в каждой особи хранить сколько осталось на складе
        public List<int> orderIn = new List<int>(); //в каждой особи хранить сколько ещё нужно клиенту
        public Osob(Gene g)
        {
            osob.Add(g);
        }
        public Osob()
        {
        }
        public Osob(int N, int M)
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    Gene m = new Gene(i, j, 0);
                    osob.Add(m);
                }
            }
        }

        public Osob(List<Gene> lst)
        {
            osob = new List<Gene>();
            for (int i = 0; i < lst.Count; i++)
            {
                osob.Add(lst[i]);
            }
        }

        public bool SequenceEquals(Osob osb)
        {
            for (int i = 0; i < osb.osob.Count; i++)
            {
                if (osb.osob[i].size != this.osob[i].size)
                {
                    return false;
                    //break;
                }
            }
            return true;
        }

        public void PrintOsob(int N, int M, string path, int[,] matrV, RichTextBox rch) //печать особи в файл
        {
            StreamWriter sw;
            FileInfo fi = new FileInfo(path);
            sw = fi.AppendText();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    sw.Write(osob[M * i + j].size + "   ");
                }
                sw.WriteLine();
            }
            sw.Write("Стоимость перевозки по данному маршруту: " + cost(matrV));
            sw.WriteLine();
            sw.Close();
        }

        public void PrintOsobRtb(int N, int M, int[,] matrV, RichTextBox rch) //печать особи в окно
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    rch.Text += osob[M * i + j].size + "   ";
                }
                rch.Text += "\n";
            }
            rch.Text += "\n";
            rch.Text += "Стоимость перевозки по данному маршруту: " + cost(matrV);

        }

        public void CreateOsob(List<int> lstStock, List<int> lstOrder, int N, int M) //создание новой рандомной особи
        {
            List<int> lstSt = new List<int>(lstStock); //количество товаров на складе
            List<int> lstOr = new List<int>(lstOrder); //количество запросов каждого клиента
            Random rand = new Random();
            for (int i = 0; i < lstSt.Count; i++)
            {
                for (int j = 0; j < lstOr.Count; j++)
                {
                    // int z = rand.Next();
                    Gene m = new Gene(i, j, -1); // устанавливаем все числа непомеченными
                    osob.Add(m); //rand.Next(j*(255/M)-(255/M), j*(255/M))
                }

            }
            List<int> metka = new List<int>();
            int q = -1;
            while (metka.Count != N * M) // пока весь список генов не будет помечен
            {
                while (metka.Contains(q) || q == -1) { q = rand.Next(0, N * M); }
                metka.Add(q);
                int ii = osob[q].stock;
                int jj = osob[q].order;
                int val; if (lstSt[ii] < lstOr[jj]) val = lstSt[ii]; else val = lstOr[jj];
                osob[q].size = val;
                lstSt[ii] -= val;
                lstOr[jj] -= val;
            }
        }

        public void addition(List<int> lstStock, List<int> lstOrder, int N, int M, int point) //дозаполнение неполной особи
        {
            List<int> lstSt = new List<int>();
            List<int> lstOr = new List<int>();
            Random rand = new Random();
            lstSt = remainderStock(lstStock);
            lstOr = remainderOrder(lstOrder);
            List<int> metka = new List<int>();
            int q = -1;
            while (metka.Count != (N * M - point)) // пока весь список генова минус гены уже распределенные не будет помечен
            {
                while (metka.Contains(q) || q == -1) { q = rand.Next(point, N * M); }
                metka.Add(q);
                int ii = osob[q].stock;
                int jj = osob[q].order;
                int val; if (lstSt[ii] < lstOr[jj]) val = lstSt[ii]; else val = lstOr[jj];
                osob[q].size = val;
                lstSt[ii] -= val;
                lstOr[jj] -= val;
            }
            while (lstOr.Sum() > 0 || lstSt.Sum() > 0)
            {
                List<int> metka2 = new List<int>();
                int q2 = -1;
                while (metka2.Count != (N * M))
                {
                    while (metka2.Contains(q2) || q2 == -1) { q2 = rand.Next(N * M); }
                    metka2.Add(q2);
                    int ii = osob[q2].stock;
                    int jj = osob[q2].order;
                    int val;
                    if (lstSt[ii] != 0 && lstOr[jj] != 0)
                    {
                        if (lstSt[ii] < lstOr[jj]) val = lstSt[ii]; else val = lstOr[jj];
                        osob[q2].size += val;
                        lstSt[ii] -= val;
                        lstOr[jj] -= val;
                    }
                }
            }
        }

        public void addition2(Osob osob2, List<int> lstStock, List<int> lstOrder, int N, int M, int point) //дозаполнение неполной особи
        {
            List<int> lstSt = new List<int>();
            List<int> lstOr = new List<int>();
            Random rand = new Random();
            lstSt = remainderStock(lstStock);
            lstOr = remainderOrder(lstOrder);
            List<int> metka = new List<int>();
            int q = -1;
            for (int i = point; i < osob.Count(); i++)
            {
                int val;
                int ii = osob[i].stock;
                int jj = osob[i].order;
                if (lstOr[jj] > 0 & lstSt[ii] > 0)
                {
                    if (lstSt[ii] < lstOr[jj])
                    {
                        if (osob2.osob[i].size <= lstSt[ii])
                            val = osob2.osob[i].size;
                        else
                            val = lstSt[ii];
                    }
                    else
                    {
                        if (osob2.osob[i].size <= lstOr[jj])
                            val = osob2.osob[i].size;
                        else
                            val = lstOr[jj];
                    }
                    osob[i].size += val;
                    lstSt[ii] -= val;
                    lstOr[jj] -= val;                   
                }
            }
          
            while (lstOr.Sum() > 0 || lstSt.Sum() > 0)
            {
                List<int> metka2 = new List<int>();
                int q2 = -1;
                while (metka2.Count != (N * M))
                {
                    while (metka2.Contains(q2) || q2 == -1) { q2 = rand.Next(N * M); }
                    metka2.Add(q2);
                    int ii = osob[q2].stock;
                    int jj = osob[q2].order;
                    int val;
                    if (lstSt[ii] != 0 && lstOr[jj] != 0)
                    {
                        if (lstSt[ii] < lstOr[jj]) val = lstSt[ii]; else val = lstOr[jj];
                        osob[q2].size += val;
                        lstSt[ii] -= val;
                        lstOr[jj] -= val;
                    }
                }
            }
        }


        public void additionMutO(List<int> lstStock, List<int> lstOrder, int N, int M, int s, int o1, int o2) //дозаполнение мутирующей особи
        {
            List<int> lstSt = new List<int>();
            List<int> lstOr = new List<int>();

            Random rand = new Random();

            for (int i = 0; i < osob.Count; i++) // все элементы двух столбцов помечаем -1
            {
                if (osob[i].stock != s)
                {
                    if ((osob[i].order == o1) || (osob[i].order == o2))
                        osob[i].size = 0;
                }
            }
            lstSt = remainderStock(lstStock);
            lstOr = remainderOrder(lstOrder);


            while (lstOr.Sum() > 0 || lstSt.Sum() > 0)
            {
                List<int> metka2 = new List<int>();
                int q2 = -1;
                while (metka2.Count != (N * M))
                {
                    while (metka2.Contains(q2) || q2 == -1) { q2 = rand.Next(N * M); }
                    metka2.Add(q2);
                    int ii = osob[q2].stock;
                    int jj = osob[q2].order;
                    int val;
                    if (lstSt[ii] != 0 && lstOr[jj] != 0)
                    {
                        if (lstSt[ii] < lstOr[jj]) val = lstSt[ii]; else val = lstOr[jj];
                        osob[q2].size += val;
                        lstSt[ii] -= val;
                        lstOr[jj] -= val;
                    }
                }
            }
        }

        public List<int> SumStr(int N) //сумма товаров отданных складом
        {
            int sum;
            List<int> sumStr = new List<int>();
            for (int i = 0; i < N; i++)
            {
                sum = 0;
                for (int j = 0; j < osob.Count; j++)
                {
                    if (osob[j].stock == i)
                    {
                        if (osob[j].size == -1) sum += 0;
                        else
                            sum = sum + osob[j].size;
                    }
                }
                sumStr.Add(sum);
            }
            return sumStr;
        }

        public List<int> SumKol(int M) //сумма товаров, которые доставлены клиенту
        {
            int sum;
            List<int> sumKol = new List<int>();
            for (int i = 0; i < M; i++)
            {
                sum = 0;
                for (int j = 0; j < osob.Count; j++)
                    if (osob[j].order == i)
                    {
                        if (osob[j].size == -1) sum += 0; //если это из неполной особи с метками
                        else
                            sum = sum + osob[j].size;
                    }
                sumKol.Add(sum);
            }
            return sumKol;
        }

        public int cost(int[,] matrV) //целевая функция, вся стоимость перевозок
        {
            int cost = 0;

            for (int i = 0; i < osob.Count(); i++)
            {
                if (osob[i].size != 0) cost = cost + osob[i].size * matrV[osob[i].stock, osob[i].order];
            }
            return cost;
        }

        public List<int> remainderStock(List<int> lstStock) //возвращает список коливества товара на складе, который не распределён
        {
            List<int> remSt = new List<int>();
            List<int> stk = new List<int>(lstStock);
            remSt = SumStr(stk.Count); //сумма товаров отданных складом
            for (int i = 0; i < stk.Count; i++)
            {
                remSt[i] = stk[i] - remSt[i];
            }
            return remSt;
        }

        public List<int> remainderOrder(List<int> lstOrder) //возвращает список коливества товара, который ещё не доставили клиенту
        {
            List<int> remOr = new List<int>();
            List<int> ord = new List<int>(lstOrder);
            remOr = SumKol(ord.Count);
            for (int i = 0; i < ord.Count; i++)
            {
                remOr[i] = ord[i] - remOr[i];
            }
            return remOr;
        }


    }
}
