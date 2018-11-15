using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Pract
{

    public partial class Form1 : Form
    {
        int n1,n2;
        public Form1()
        {
            InitializeComponent();
            n1 = 0;
            n2 = 0;
            stockW = new List<int>();//N
            orderW = new List<int>(); //M

        }

        Random rand = new Random();
        int N, M;
        int cros, mut, end, kGen, priceMin, priceMax;
        int[,] MatrVesov;
        List<int> stockW;//N
        List<int> orderW; //M

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e) //заполняем потребности клиетов
        {
            M = Convert.ToInt32(textBox2.Text);
            if (e.KeyCode == Keys.Enter)
            {
                n1 = n1 + 1;
                if (n1 <= M)
                {
                    orderW.Add(Convert.ToInt32(textBox3.Text));
                    textBox3.Text = "";
                }
                else
                {
                    MessageBox.Show("вы заполнили всех клиентов");
                    textBox3.Clear();
                    textBox3.Enabled = false;
                }
            }
        }

        string path = "pract.txt";



        private void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            N = Convert.ToInt32(textBox1.Text); //количество складов - A сверху вних
            M = Convert.ToInt32(textBox2.Text); //количество потребителей - В слева направо
            cros = Convert.ToInt32(textBox6.Text);
            mut = Convert.ToInt32(textBox7.Text);
            end = Convert.ToInt32(textBox8.Text); //критерий остановки
            kGen = Convert.ToInt32(textBox9.Text); //количество особей в поколении         
    //        priceMin = Convert.ToInt32(textBox10.Text);
      //      priceMax = Convert.ToInt32(textBox11.Text);

           
          orderW.Add(25); stockW.Add(150);
           orderW.Add(200); stockW.Add(15);
            orderW.Add(100); stockW.Add(70);
            orderW.Add(5); stockW.Add(15);
            orderW.Add(20); stockW.Add(100);
      //    stockW.Add(50); 

            int sumStock = stockW.Sum(); 
            int sumOrder = orderW.Sum();

            bool addS =false;
            bool addO=false;
        /*    if (stockW.Sum() != orderW.Sum())
            { //задача является открытой
                int a = 0;
                if (sumStock > sumOrder)
                {
                    a = sumStock - sumOrder;
                    orderW.Add(a);
                    M++; 
                    addO=true;
                }
                else
                {
                    a = sumOrder - sumStock;
                    stockW.Add(a);
                    N++;
                    addS=true;
                }
            }
            */
                MatrVesov = new int[N, M];
            //for (int i = 0; i < N; i++) //формируем матрицу весов
            //{
            //    for (int j = 0; j < M; j++)
            //    {
            //        int r = rand.Next(1, 9);
            //        MatrVesov[i, j] = r;
            //}
            //}
            MatrVesov[0, 0] = 2; MatrVesov[0, 1] = 1; MatrVesov[0, 2] = 7; MatrVesov[0, 3] = 3; MatrVesov[0, 4] = 3;
            MatrVesov[1, 0] = 5; MatrVesov[1, 1] = 8; MatrVesov[1, 2] = 5; MatrVesov[1, 3] = 1; MatrVesov[1, 4] = 5;
            MatrVesov[2, 0] = 4; MatrVesov[2, 1] = 5; MatrVesov[2, 2] = 4; MatrVesov[2, 3] = 1; MatrVesov[2, 4] = 6;
            MatrVesov[3, 0] = 6; MatrVesov[3, 1] = 2; MatrVesov[3, 2] = 6; MatrVesov[3, 3] = 4; MatrVesov[3, 4] = 8;
            MatrVesov[4, 0] = 2; MatrVesov[4, 1] = 2; MatrVesov[4, 2] = 3; MatrVesov[4, 3] = 9; MatrVesov[4, 4] = 8;

            //if (addO)
            //{
            //    int j = M - 1;
            //    for (int i = 0; i < N; i++)
            //        MatrVesov[i, j] = 0;
            //}

            //if (addS)
            //{
            //    int i = N - 1;
            //    for (int j = 0; j < M; j++)
            //        MatrVesov[i, j] = 0;
            //}
            PrintStockOrder(stockW, orderW);
            PrintMatrV(MatrVesov); //выводим матрицу весов
        
            List<Osob> generation = new List<Osob>(); //поколение, состоит из нескольких расписаний
            StreamWriter sw;
            FileInfo fi = new FileInfo(path);
            sw = fi.AppendText();
            sw.Write("Начальное поколение: ");
            sw.WriteLine();
            sw.Close();

            for (int j = 0; j < kGen; j++)
            { //заполняем поколение особями                              
                Osob osbb = new Osob();
                osbb.CreateOsob(stockW, orderW, N, M);
                osbb.PrintOsob(N, M, path, MatrVesov,richTextBox1);
                generation.Add(osbb);
            }


      //      GeneticAlgorithm.Crossover(generation, path, N, M, cros, mut, MatrVesov, stockW, orderW, richTextBox1);

            Osob min = new Osob();
            min =GeneticAlgorithm.osobWhithMinCost(generation, MatrVesov);
            richTextBox1.Text+="\nМинимальная стоимость перевозки в начальном поколении = "+min.cost(MatrVesov)+"\n";
            int k = 0;
            int kol=0;
            Osob min1 = new Osob();
            Osob min2 = new Osob();
            do {
                min1 = GeneticAlgorithm.osobWhithMinCost(generation, MatrVesov); //находим мин. особь в поколении
                //обращаемся к кроссоверу, в нём же и мутацию реализовать
                GeneticAlgorithm.Crossover(generation, path, N, M, cros,mut,  MatrVesov, stockW, orderW,richTextBox1);
                //печатаем новое поколение с двумя потомками
                kol++;
                sw = fi.AppendText();
                sw.Write(kol+" поколение");
                sw.WriteLine();
                sw.Close();
                //печать переполненного поколения
                for (int j = 0; j < generation.Count(); j++)
                {
                    generation[j].PrintOsob(N, M, path, MatrVesov,richTextBox1);
                }
                //обращаемся к редукции                            
                do GeneticAlgorithm.reduction(generation, MatrVesov);
                while (generation.Count() != kGen);

                min2 = GeneticAlgorithm.osobWhithMinCost(generation, MatrVesov);

                if (min1.SequenceEquals(min2)) //сравниваем по значению предыдущую мин.особь и мин. особь нового поколения
                    k++; //если равны
                else k = 0;
                  } while (k != end);  

             min1 = GeneticAlgorithm.osobWhithMinCost(generation, MatrVesov);
            richTextBox1.Text += "Лучшая особь: \n";
            min1.PrintOsobRtb(N, M, MatrVesov, richTextBox1);
            min1.PrintOsob(N, M, path, MatrVesov, richTextBox1);
       //     richTextBox1.Text += "\nСтоимость перевозки  = " + min1.cost(MatrVesov) + "\n"; 

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e) //заполняем содержимое складов
        {
            N = Convert.ToInt32(textBox1.Text);
            if (e.KeyCode == Keys.Enter)
            {
                n2 += 1;
                //        richTextBox1.Text += n2;
                if (n2 <= N)
                {
                    stockW.Add(Convert.ToInt32(textBox4.Text));
                    textBox4.Text = "";
                }
                else { MessageBox.Show("Вы заполнили все склады"); textBox4.Enabled = false; textBox4.Clear(); }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
   


       void PrintMatrV(int[,] arr)
        {
            StreamWriter sw;
            FileInfo fi = new FileInfo(path);
            sw = fi.AppendText();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    richTextBox1.Text += arr[i, j] + " ";
                    sw.Write(arr[i, j] + " ");
                }
                richTextBox1.Text += "\n";
                sw.WriteLine();
            }
            sw.Close();
            richTextBox1.Text += "\n";
        }

       void PrintStockOrder(List <int> stock, List<int> order) //вывод загруженности складов/клиентов
        {
            StreamWriter sw;
            FileInfo fi = new FileInfo(path);
            sw = fi.AppendText();
            sw.Write("     ");
            richTextBox1.Text += "     ";
            for (int j = 0; j < M; j++)
            {
                richTextBox1.Text += order[j] + " ";
                sw.Write(order[j] + " ");
            }
            sw.WriteLine();
            richTextBox1.Text +="\n";
            for (int i = 0; i < N; i++)
            {
                richTextBox1.Text += stock[i]+"\n";
                sw.Write(stock[i]);
                sw.WriteLine();
            }
            sw.Close();
            richTextBox1.Text += "\n";
        }

       List<int> SumStr(Osob osob)
        {
            int sum;
            List<int> sumStr = new List<int>();
            for (int i = 0; i < N; i++)
            {
                sum = 0;
                for (int j = 0; j < osob.osob.Count; j++)
                    if (osob.osob[j].stock == i) sum =sum+ osob.osob[j].size;
                sumStr.Add(sum);
            }
            return sumStr;
        }

       
        void PrintOsob(Osob osob)
        {
            StreamWriter sw;
            FileInfo fi = new FileInfo(path);
            sw = fi.AppendText();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    richTextBox1.Text += osob.osob[M*i+j].size + "   ";
                    sw.Write(osob.osob[M * i + j].size+"   ");                 
                }
                richTextBox1.Text += "\n";
                sw.WriteLine();
            }
            sw.Write("Стоимость перевозки по данному маршруту: " + osob.cost(MatrVesov));
            richTextBox1.Text += "Стоимость перевозки по данному маршруту: "+ osob.cost(MatrVesov);
            richTextBox1.Text += "\n";
            sw.WriteLine();
            sw.Close();
        }

    }
}

