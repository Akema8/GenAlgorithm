using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Pract
{
    static class GeneticAlgorithm
    {
        public static void Crossover(List<Osob> generation, string path, int N, int M, int cros, int mut, int[,] matrV, List<int> lstStock, List<int> lstOrder, RichTextBox rch)
        {
            Random rand = new Random();
            int k = generation.Count();
            int parent1, parent2;
            parent1 = rand.Next(k);
            do parent2 = rand.Next(k);
            while (parent1 == parent2);
            StreamWriter sw;
            FileInfo fi = new FileInfo(path);
            sw = fi.AppendText();
            sw.WriteLine("Для кроссовера выбраны особи: " + parent1 + " и " + parent2);

            int point = rand.Next(1, N * M);  //точка разбиения 

            sw.WriteLine("Точка разбиения = " + point);
            sw.WriteLine("Родительские особи:");
            sw.Close();

            generation[parent1].PrintOsob(N, M, path, matrV, rch);
            generation[parent2].PrintOsob(N, M, path, matrV, rch);

            if (rand.Next(0, 100) <= cros) //будет кроссовер или нет
            {
                sw = fi.AppendText();
                sw.WriteLine("Для данных особей кроссовер произошёл");
                sw.Close();
                Osob child1 = new Osob(N, M);
                Osob child2 = new Osob(N, M);
                for (int i = 0; i < generation[parent1].osob.Count; i++)
                {
                    if (i < point)
                    {
                        child1.osob[i].size = generation[parent1].osob[i].size;
                        child2.osob[i].size = generation[parent2].osob[i].size;
                    }
                }
                //     child1.addition(lstStock, lstOrder, N, M, point);
                //     child2.addition(lstStock, lstOrder, N, M, point);
                child1.addition2(generation[parent2],lstStock, lstOrder, N, M, point);
                child2.addition2(generation[parent1], lstStock, lstOrder, N, M, point);
                sw = fi.AppendText();
                sw.WriteLine("Первый потомок: ");
                sw.Close();
                child1.PrintOsob(N, M, "pract.txt", matrV, rch);
                sw = fi.AppendText();
                sw.WriteLine("Второй потомок: ");
                sw.Close();
                child2.PrintOsob(N, M, "pract.txt", matrV, rch);


                if (rand.Next(0, 100) <= mut) //будет мутация над потомком
                {
                    sw = fi.AppendText();
                    sw.WriteLine("Произошла мутация первого потомка ");
                    sw.Close();
                    MutationOrder(child1, N, M, lstStock, lstOrder);
                }
                if (rand.Next(0, 100) <= mut) //будет мутация над потомком
                {
                    sw = fi.AppendText();
                    sw.WriteLine("Произошла мутация второго потомка ");
                    sw.Close();
                    MutationOrder(child2, N, M, lstStock, lstOrder);
                }
                //добавляем потомков 
                generation.Add(child1);
                generation.Add(child2);
            }
        }

        public static void reduction(List<Osob> generation, int[,] matrV) //редукция
        {
            List<int> func = new List<int>();
            for (int i = 0; i < generation.Count(); i++)
            {
                func.Add(generation[i].cost(matrV));
            }
            generation.Remove(generation[func.IndexOf(func.Max())]); //удаляем особь с наибольшим значением целевой функции
        }

        public static Osob osobWhithMinCost(List<Osob> generation, int[,] matrV) //находим особь с минимальной ц.ф.
        {
            List<int> func = new List<int>();
            for (int i = 0; i < generation.Count(); i++)
            {
                func.Add(generation[i].cost(matrV));
            }

            return generation[func.IndexOf(func.Min())]; //возвращаем особь с минимальным ц.ф.
        }


        public static void MutationOrder(Osob osob, int N, int M, List<int> stockW, List<int> orderW) //мутация по клиенту
        {
            Random rand = new Random();
            int num = rand.Next(osob.osob.Count);//выбор гена над которым будет мутация
            int bit = rand.Next(7); //выбор мутирующего бита
            int ord1 = osob.osob[num].order; //номер клиента до мутации
            int ord2; //номер клиента после мутации
            int y = osob.osob[num].order * (255 / M); //представление числа в размере 0-255 
            int z = Convert.ToInt32(Convert.ToString(y, 2)); //перевод в двоичную систему
            //         sw.WriteLine(z);
            int res = InvertBit(z, bit); //инвертируем выбранный бит
            ord2 = ConvertToDec(res.ToString()) / (255 / M); //новый номер клиента для этого гена
            StreamWriter sw;
            FileInfo fi = new FileInfo("pract.txt");

            if (ord1 != ord2)
            {
                int i = osob.osob[num].stock; //osob.osob.Count()/N*i-M+ord2 номер склада в выбранном гене
                int test;
                test = M * i + ord2;

                sw = fi.AppendText();
                sw.WriteLine("Столбец " + ord1 + " и " + ord2 + " строка " + i + " Элементы:" + num + " " + test);
                sw.Close();

                if ((osob.osob[num].size == 0) && (osob.osob[test].size != 0)) //если размер гена 0, а у второго нет, то меняем их местами, чтобы не было два нуля, когда нулем заменяем число.
                {
                    int tm = num;
                    num = test;
                    test = tm;
                    tm = ord2;
                    ord2 = ord1;
                    ord1 = tm;
                }

                int prov = orderW[ord2];
                if (orderW[ord2] >= osob.osob[num].size) //если запрос клиента >= передаваемому размеру
                {
                    osob.osob[test].size = osob.osob[num].size; //вставляем в х ген размер мутировавшего
                    osob.osob[num].size = 0;
                    sw = fi.AppendText();
                    sw.WriteLine("Столбец " + ord1 + " и " + ord2 + " строка " + i + " Элементы:" + num + " " + test);
                    sw.Close();
                    osob.additionMutO(stockW, orderW, N, M, i, ord1, ord2);
                }
                else
                {
                    osob.osob[test].size = orderW[ord2];
                    osob.osob[num].size -= orderW[ord2]; //вычтем на величину перенесенную 

                    sw = fi.AppendText();
                    sw.WriteLine("**Столбец " + ord1 + " и " + ord2 + " строка " + i);
                    sw.WriteLine("Запрос клиента меньше, чем предлагалось переместить в клетку"); //т.е. в мутировавшей клетке остаток
                    sw.Close();
                    osob.additionMutO(stockW, orderW, N, M, i, ord1, ord2);
                }
            }
        }

        public static void MutationStock(Osob osob, int N, int M) //мутация по складу
        {
            Random rand = new Random();
            int num = rand.Next(osob.osob.Count);//выбор гена над которым будет мутация
            int bit = rand.Next(7); //выбор мутирующего бита
            int stk1 = osob.osob[num].stock;
            int stk2;
            int z = Convert.ToInt32(Convert.ToString(osob.osob[num].stock, 2));
            int res = InvertBit(z, bit); //инвертируем выбранный бит
            stk2 = ConvertToDec(res.ToString()); //новый номер склада для этого гена
            if (stk1 != stk2)
            {
                int j = osob.osob[num].order;
                int x = osob.osob.Count / N * stk2 + j - 1;
                osob.osob[x].size += osob.osob[num].size; //перекидываем на другой склад товар
                osob.osob[num].size = 0; //обнуляем товар у этого склада
                                         //пересчитваем особь
            }
        }

        private static int ConvertToDec(string str)
        {
            int res = 0;
            int k = 0;
            int i = str.Length;
            while (i > 0)
            {
                res = res + Convert.ToInt32(str[k].ToString()) * Convert.ToInt32(Math.Pow(2, i - 1));
                k++;
                i--;
            }

            return res;
        }

        private static int InvertBit(int z, int k)//инвертирование бита при мутации  
        {
            int res = 0;
            string str = Convert.ToString(z);
            string strres = "";
            //      Random rand = new Random();
            //     int k = rand.Next(0, 7);

            for (int i = 0; i < str.Length; i++)
            {
                if (i != k) strres = strres + str[i];
                else
                {
                    if (str[k] == '0') strres = strres + '1';
                    if (str[k] == '1') strres = strres + '0';
                }
            }
            res = Convert.ToInt32(strres, 10);
            return res;
        }

    }
}
