using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codHammingaCs
{
    abstract class Hamming_Base
    {
        protected Hamming_Base(byte[] входнойВектор)
        {
            вычислитьРазмерМатрицы(входнойВектор);
            создатьВременныйВектор(входнойВектор);
            создатьМатрицуПреобразования();
        }
        protected int строк, столбцов, проверочных_ячеек;
        protected byte[,] матрица_преобразования;
        protected byte[] временныйВектор;
        protected void вычислитьРазмерМатрицы(byte[] входнойВектор)
        {
            проверочных_ячеек = количествоПроверочныхЯчеек(входнойВектор);
            строк_и_столбцов(входнойВектор);
        }
        abstract protected void строк_и_столбцов(byte[] вектор);
        abstract protected int количествоПроверочныхЯчеек(byte[] вектор);
        protected void создатьМатрицуПреобразования()
        {
            матрица_преобразования = new byte[строк, столбцов];
            for (int i = 0; i < столбцов; i++)
            {
                string строка = Convert.ToString(i + 1, 2);
                int номер_строки = 0;
                if (строка.Length == 1) матрица_преобразования[номер_строки, i] = 1;
                else
                    for (int j = строка.Length; j != 0; j--)
                    {
                        char ch = строка[j - 1];
                        if (ch == '1') матрица_преобразования[номер_строки, i] = 1;
                        номер_строки++;
                    }
            }
        }
        abstract protected void создатьВременныйВектор(byte[] вектор);
        protected void умножитьВекторНаМатрицу(ref byte[] вектор_столбец)
        {
            for (int i = 0; i < строк; i++)
            {
                for (int j = 0; j < столбцов; j++)
                {
                    вектор_столбец[i] += (byte)(матрица_преобразования[i, j] * временныйВектор[j]);
                }
                вектор_столбец[i] = (byte)(вектор_столбец[i] % 2);
            }
        }
    }
    class Hamming_Coder : Hamming_Base
    {
        public Hamming_Coder(byte[] входнойВектор) : base(входнойВектор) { }
        protected override int количествоПроверочныхЯчеек(byte[] вектор)
        {
            int количество_контрольных_бит = 0;
            while (количество_контрольных_бит > Math.Pow(2, количество_контрольных_бит) - вектор.Length - 1)
                количество_контрольных_бит++;
            //количество_контрольных_бит++;
            return количество_контрольных_бит;// (int)Math.Ceiling(Math.Log(вектор.Length + Math.Log(вектор.Length, 2), 2));
        }
        protected override void строк_и_столбцов(byte[] вектор)
        {
            строк = проверочных_ячеек;
            столбцов = вектор.Length + проверочных_ячеек;
            временныйВектор = new byte[столбцов];
        }
        protected override void создатьВременныйВектор(byte[] вектор)
        {
            int счетчик = 0;
            for (int i = 0; i < столбцов; i++)
            {
                if ((i + 1).Equals((int)Math.Pow(2, счетчик)))
                {
                    временныйВектор[i] = 0;
                    счетчик++;
                }
                else
                {
                    временныйВектор[i] = вектор[i - счетчик];
                }
            }
        }
        public byte[] закодировать()
        {
            byte[] вектор_столбец = new byte[проверочных_ячеек];
            умножитьВекторНаМатрицу(ref вектор_столбец);
            int итератор = 0;
            byte[] результат = new byte[столбцов];
            for (int i = 0; i < столбцов; i++)
            {
                if ((i + 1).Equals((int)Math.Pow(2, итератор)))
                {
                    результат[i] = вектор_столбец[итератор];
                    итератор++;
                }
                else
                    результат[i] = временныйВектор[i];
            }
            return результат;
        }
    }
    //class Hamming_Decoder : Hamming_Base
    //{
    //    public Hamming_Decoder(byte[] входной_вектор) : base(входной_вектор) { }
    //    protected override void строк_и_столбцов(byte[] вектор)
    //    {
    //        строк = проверочных_ячеек;
    //        столбцов = вектор.Length;
    //        // - проверочных_ячеек];
    //    }
    //    protected override int количествоПроверочныхЯчеек(byte[] вектор)
    //    {
    //        double длинаВектора = (double)вектор.Length;
    //        int счетчик = 0;
    //        while (длинаВектора >= 2)
    //        {
    //            длинаВектора /= 2;
    //            счетчик++;
    //        }
    //        счетчик++;
    //        return счетчик;
    //    }
    //    protected override void создатьВременныйВектор(byte[] вектор)
    //    {
    //        временныйВектор = вектор;
    //    }
    //    public byte[] декодировать()
    //    {
    //        byte[] вектор_синдромов = new byte[проверочных_ячеек];
    //        Random ran = new Random();
    //        int num = ran.Next(0, временныйВектор.Length);
    //        if (временныйВектор[num] == 0)
    //            временныйВектор[num] = 1;
    //        else
    //            временныйВектор[num] = 0;
    //        умножитьВекторНаМатрицу(ref вектор_синдромов);
    //        string s = "";
    //        for (int i = (проверочных_ячеек - 1); i > -1; i--)
    //            s += вектор_синдромов[i].ToString();
    //        int позиция_ошибки = Convert.ToInt32(s, 2);
    //        if (позиция_ошибки > 0)
    //        {
    //            if (временныйВектор[позиция_ошибки-1] == 0)
    //                временныйВектор[позиция_ошибки-1] = 1;
    //            else
    //                временныйВектор[позиция_ошибки-1] = 0;
    //        }
    //        int счетчик = 0;
    //        byte[] результат = new byte[временныйВектор.Length - проверочных_ячеек];
    //        for (int i = 0; i < временныйВектор.Length; i++)
    //        {
    //            if (!((i + 1).Equals((int)Math.Pow(2, счетчик))))
    //            {
    //                результат[i - счетчик] = временныйВектор[i];

    //            }
    //            else
    //            {
    //                счетчик++;
    //            }
    //        }
    //        return результат;
    //    }
    //}
}

//    class Hamming_Coder
//    {
//        public Hamming_Coder(byte []array)
//        {
//            arrrr = array;
//            arr = созданиеМатрицыПреобразования(array);
//             byte [] v =закодировать();
//        }
//        byte[] arrrr;
//        private int strok, stolbiki, addCells;
//        private  byte[,] arr;
//        public byte[] закодировать() 
//        {
//            byte[] vec = new byte[addCells];
//            for (int i = 1; i < strok; i++)
//            {
//                for (int j = 0; j < stolbiki; j++)
//                {
//                    vec[i-1] += arr[i, j];
//                }
//                vec[i-1] = (byte)((int)vec[i-1] % 2);
//            }
//            int iter1 = 0;
//            byte [] res = new byte[stolbiki];
//            //for (int i = 0; i < stolbiki; i++) res[i] = arr[0, i];
//                for (int i = 0; i < stolbiki; i++)
//                {
//                    if ((i + 1).Equals((int)Math.Pow(2, iter1)))
//                    {
//                        res[i] = vec[iter1];
//                        iter1++;
//                    }
//                    else
//                        res[i] = arr[0, i];
//                }

//            return res;
//        }
//        private void раскодировать()
//        {

//        }
//        private int количествоПроверочныхЯчеек(byte [] array) 
//        {
//            return (int)Math.Round(Math.Log(array.Length, 2),MidpointRounding.ToEven);
//        }

//        private byte[,] созданиеМатрицыПреобразования(byte[] array)
//        {
//            addCells = количествоПроверочныхЯчеек(array);
//            strok = addCells + 1;
//            stolbiki = array.Length + addCells;
//            byte[,] arr = new byte[strok, stolbiki];
//            int iter1 = 0;
//            //Добавление доролнительных битов 
//            for (int i = 0; i < stolbiki; i++)
//            {
//                if ((i + 1).Equals((int)Math.Pow(2, iter1)))
//                {
//                    arr[0, i] = 0;
//                    iter1++;
//                }
//                else 
//                {
//                    arr[0, i] = array[i-iter1];
//                }
//            }

//            //Заполнение матрицы по алгоритму
//            stolbiki = arr.GetLength(1);
//            strok = arr.GetLength(0);
//            //for (int i = 0; i < strok; i++)
//            //{
//            //    for (int j = 0; j < stolbiki; j++)
//            //    {
//            //        Console.Write(arr[i, j]);
//            //        Console.Write(" ");
//            //    }
//            //    Console.WriteLine();
//            //}
//            //Console.WriteLine();
//            for (int i = 0; i < stolbiki; i++)
//            {
//                string str = Convert.ToString(i + 1, 2);
//                int h = 1;
//                if (str.Length == 1) arr[h, i] = 1;
//                else
//                    for (int j = str.Length; j != 0; j--)
//                    {
//                        char ch = str[j - 1];
//                        if (ch == '1') arr[h, i] = 1;
//                        h++;
//                    }
//                //for (int j = 0; j < strok; j++)
//                //{
//                //    for (int p = 0; p < stolbiki; p++)
//                //    {
//                //        Console.Write(arr[j, p]);
//                //        Console.Write(" ");
//                //    }
//                //    Console.WriteLine();
//                //}
//            }
//                return arr;
//        }
//    }
//    class Hamming_Decoder 
//    {
//        public Hamming_Decoder()
//        {

//        }
//        private byte[,] arr;
//        private int stolbiki;
//        private int strok;
//        public byte[] Декодировать(byte [] vec)
//        {
//            for (int i = 0; i < stolbiki; i++)
//            {
//                string str = Convert.ToString(i + 1, 2);
//                int h = 0;
//                if (str.Length == 1) arr[h, i] = 1;
//                else
//                    for (int j = str.Length; j != 0; j--)
//                    {
//                        char ch = str[j - 1];
//                        if (ch == '1') arr[h, i] = 1;
//                        h++;
//                    }
//                //for (int j = 0; j < strok; j++)
//                //{
//                //    for (int p = 0; p < stolbiki; p++)
//                //    {
//                //        Console.Write(arr[j, p]);
//                //        Console.Write(" ");
//                //    }
//                //    Console.WriteLine();
//                //}
//            }
//            return vec;
//        }
//    }
//}
