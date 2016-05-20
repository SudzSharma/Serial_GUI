using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using ZedGraph;
using System.Globalization;

namespace Serial
{
    public partial class Form1 : Form
    {
        int a = 0;
        int temp = 0;
        String Inputdata = "";
       // char Inputdata;
        double data = 0;
        double win = 0;
        double[] array = new double[1000000];
        int flag = 0;
        long index = 1;
        double rate = 100;
        int serialflag = 0;
        int comboint = 0;
        int ascii = 0;
        int deci = 0;
        int hex = 0;
        int Input_int;

        public Form1()
        {
            serialPort1 = new SerialPort();

            InitializeComponent();
            portnames();

            button1.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;

            textBox3.Text = "";

            GraphPane graph = zedGraphControl1.GraphPane;
            graph.Title.Text = "Serial_Data vs Time";
            graph.XAxis.Title.Text = "Time";
            graph.YAxis.Title.Text = "Serial_Data";

            PointPairList rollBuff = new PointPairList();


        }




        void portnames()
        {
            String[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);

        }





        private void button1_Click(object sender, EventArgs e)
        {
            a = 1;


            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;

            for (int i = 0; i <= 100; i++)
            {
                progressBar1.Value = i;
            }

            if (comboBox1.Text != "" && a == 1&&comboBox2.Text!="")
            {
                a++;
                comboint = Int32.Parse(comboBox2.Text);
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = 9600;
                serialPort1.DataBits = 8;
                serialPort1.Parity = 0;
                serialPort1.ReadTimeout = (comboint * 1000);
                serialPort1.Open();

            }
        }





        private void button2_Click(object sender, EventArgs e)
        {
           
            rate = Convert.ToDouble(comboBox3.Text);
            timer1.Interval = Int32.Parse(comboBox3.Text);
            win = rate / 1000.0;
            if (comboBox2.Text != "" && comboBox3.Text != "")
            {
                button1.Enabled = true;
            }



        }





        private void button3_Click(object sender, EventArgs e)
        {
            if(flag==0)
            printdata();

            button5.Enabled = false;
        }





        void printdata()
        {
            timer1.Start();
        }


        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if ((rate * index) < (comboint*1000) && serialflag == 0) 
            {

                try
                {
                    if (deci == 1)
                    {
                        Input_int=serialPort1.ReadByte();
                        Inputdata=Convert.ToString(Input_int);
                        textBox3.Text = textBox3.Text + Inputdata + Environment.NewLine;

                    
                        try
                        {


                            data = Convert.ToDouble(Inputdata);
                            array[index] = data;

                            if (index != 0 && (array[index] - array[index - 1]) > 10000)
                            {
                                array[index] = 0;
                                Console.Write(index);

                            }
                            index++;
                        }
                        catch (System.FormatException)
                        {

                            array[index] = 0;

                            index++;

                        }

                        temp++;

                        if (temp == 30 && flag == 1)
                        {
                            plotGraph();
                            temp = 0;
                        }

                    }




                    if(ascii==1)
                    {
                        int d;
                        
                            Input_int=serialPort1.ReadByte();
                            Inputdata = Convert.ToString(Input_int);
                       try
                        {
                            d = Convert.ToInt32(Inputdata);
                        }
                        catch (System.FormatException)
                        {
                            d = 46;

                        }


                        char c;

                        try
                        {
                             c = Convert.ToChar(d);
                        }
                        catch(System.OverflowException )
                        {
                            c = '.';
                        }

                        textBox3.Text = textBox3.Text + c + Environment.NewLine;

                    }
                


                    
                    if(hex==1)
                    {
                        Input_int= serialPort1.ReadByte();
                        Inputdata = Convert.ToString(Input_int);


                        int decimalNumber, quotient;
                        int i = 1, j, temp = 0;
                        char[] hexadecimalNumber = new char[100];
                        char temp1;
                        try
                        {
                            decimalNumber = int.Parse(Inputdata);
                        }
                        catch(FormatException)
                        {

                            decimalNumber = 0;
                        }

                        quotient = decimalNumber;
                        while (quotient != 0)
                        {
                            temp = quotient % 16;
                            if (temp < 10)
                                temp = temp + 48;
                            else
                                temp = temp + 55;
                            temp1 = Convert.ToChar(temp);
                            hexadecimalNumber[i++] = temp1;
                            quotient = quotient / 16;
                        }
                          char [] ch=new char[200];
                          for (j = i - 1; j > 0; j--)
                          {

                              textBox3.Text = textBox3.Text + hexadecimalNumber[j];
                          }
                          textBox3.Text = textBox3.Text + Environment.NewLine;
                      
                    }


                   


                }


                catch (TimeoutException)
                {
                    textBox3.Text = "TimeOut_Exception";
                }

            

            }
          

        }








        void plotGraph()
        {


            GraphPane graph = zedGraphControl1.GraphPane;
            graph.Title.Text = "Serial_Data vs Time";
            graph.XAxis.Title.Text = "Time";
            graph.YAxis.Title.Text = "Serial_Data";

            PointPairList rollBuff = new PointPairList();

            double m = 0;
            for (int n = 1; n < index; n++)
            {
                m = n;
                rollBuff.Add((m * win), array[n]);
            }


            Scale xScale = zedGraphControl1.GraphPane.XAxis.Scale;
            xScale.Max = m * win;
            xScale.Min =0;


            LineItem myCurve = graph.AddCurve("Data",
                     rollBuff, Color.Blue, SymbolType.Circle);

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();

        }







        private void button4_Click(object sender, EventArgs e)
        {
            serialflag = 1;
            serialPort1.Close();
        }




        private void button5_Click(object sender, EventArgs e)
        {
            flag = 1;
            printdata();
            button3.Enabled = false;
        }




        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            deci = 1;
            button3.Enabled = true;
            button5.Enabled = true;


        }

  




        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;

            ascii = 1;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;

            hex = 1;
        }

       
        private void button6_Click(object sender, EventArgs e)
        {

            serialPort1.WriteLine(textBox1.Text);

        }

        private void button7_Click(object sender, EventArgs e)
        {

            serialPort1.Write(textBox5.Text);
            serialPort1.Write(".");
            serialPort1.Write(textBox4.Text);
            serialPort1.Write(".");
            serialPort1.Write(textBox2.Text);
            serialPort1.Write(".");
        }

    
    }
}
