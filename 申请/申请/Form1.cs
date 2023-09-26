﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 申请
{
    public partial class Form1 : Form
    {
        private int YanZhengMa;
        private Timer timer = new Timer();
        //为验证码生成时间做定义
        private Timer YanZhengMaTime = new Timer();

        public const string AllPasswords = "1111111";
         
        //定义可以访问人员的工号
        public string[] EmployeeID = new string[] {"622003","621003" };
        public string machineCode = null;
        //定义申请人员的ID
        public string enteredEmployeeID = "";
        //定义保存log的位置
        private const string LogFolderPath = @"C:\Users\622003\Desktop\申请访问记录";
        private const string LogFilePath = LogFolderPath + @"\访问记录.txt";

       

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime nextHour = new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, 0, 0);
            TimeSpan timeRemaining = nextHour - now;
            textBox4.Text = $"{timeRemaining.Minutes}min {timeRemaining.Seconds}s";
        }
        /// <summary>
        /// 生成权重一的密码，每年更新一次
        /// </summary>
        /// <param name="machineCode"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GenerateYearlyKey(string machineCode, DateTime dateTime)
        {
            return dateTime.ToString("yyyy") + machineCode;
        }

       /// <summary>
       ///  生成权重二的密码，每年更新一次
       /// </summary>
       /// <param name="machineCode"></param>
       /// <param name="dateTime"></param>
       /// <returns></returns>
        public static string GenerateMonthlyKey(string machineCode, DateTime dateTime)
        {
            return dateTime.ToString("yyyyMM") + machineCode;
        }

        /// <summary>
        /// 生成权重三的密码，每小时更新一次密码
        /// </summary>
        /// <param name="machineCode"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GenerateKey(string machineCode, DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHH") + machineCode;
        }

        public static int GeneratePassword(string key)
        {
            int password = 0;
            foreach (char c in key)
            {
                password ^= c;
                password = (password << 1) | (password >> 31); // 位旋转
                password ^= (int)c * 31; // 乘质数以增加密码的复杂性
            }
            return Math.Abs(password % 100000000); // 将计算后的数字取模，以保证是8位数字的正数
        }
        /// <summary>
        /// 用来储存访问记录LOG
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="projectCode"></param>
        /// <param name="accessGranted"></param>

        public static void LogAccess(string employeeID, string projectCode, bool accessGranted)
        {
            string status = accessGranted ? "申请通过" : "申请失败";
            string logEntry = $"{DateTime.Now}: 申请ID号: {employeeID},   申请状态: {status},    申请的机台号 : {projectCode}";

            // Ensure the directory exists
            if (!Directory.Exists(LogFolderPath))
            {
                Directory.CreateDirectory(LogFolderPath);
            }

            File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
        }
        public Form1()
        {
            InitializeComponent();

            timer.Interval = 1000; // 设置定时器的时间间隔为1秒
            timer.Tick += Timer_Tick; // 添加Tick事件处理器
            //验证码每一小时刷新一次
            YanZhengMaTime.Interval = 60 * 60 * 1000;
            YanZhengMaTime.Tick += YanZhengMaTime_Tick;
            GenerateYanZhengMa();

        }
        /// <summary>
        /// 生成验证码的函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YanZhengMaTime_Tick(object sender, EventArgs e)
        {
            
            GenerateYanZhengMa();
        }

        private void GenerateYanZhengMa()
        {
            Random random = new Random();
            YanZhengMa = random.Next(10000, 100000); // 生成一个5位的随机数
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            enteredEmployeeID = textBox1.Text;

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
             machineCode = textBox2.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool accessGranted = false; // 默认为不允许访问
            string userEnteredCode = textBox5.Text;
            textBox6.Text = Convert.ToString(YanZhengMa);
            if (EmployeeID.Contains(enteredEmployeeID))
            {
                //检查此时的机台栏是否为空
                if (!string.IsNullOrEmpty(machineCode))
                {
                    //检查验证码是否正确
                    if (!string.IsNullOrEmpty(userEnteredCode) && int.Parse(userEnteredCode) == YanZhengMa)
                    {
                        GeneratePassword(machineCode);
                        label1.Text = "Pass";
                        label1.BackColor = Color.Lime;
                        // 使用当前时间和机台编号生成密码
                        //每小时生成的密码
                        int password = GeneratePassword(GenerateKey(machineCode, DateTime.Now));
                        textBox3.Text = password.ToString("D8");

                        accessGranted = true; // 允许访问

                        // 计算距离下一个整点的时间差
                        DateTime now = DateTime.Now;
                        DateTime nextHour = new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, 0, 0);
                        TimeSpan timeRemaining = nextHour - now;
                        timer.Start(); // 启动定时器
                    }
                    else
                    {
                        label1.Text = "Fail";
                        label1.BackColor = Color.Red;
                        textBox5.Text = "请输入正确的验证码";
                        return; // 如果验证码不正确，直接返回，不再执行后续代码


                    }
                }
                else
                {
                    label1.Text = "Fail";
                    label1.BackColor = Color.Red;
                    textBox3.Text = "请输入机台编号";
                    timer.Stop(); // 停止定时器
                    textBox4.Text = "";
                }
            }
            else
            {
                label1.Text = "Fail";
                label1.BackColor = Color.Red;
                textBox3.Text = "请输入正确的工号";
                timer.Stop(); // 停止定时器
                textBox4.Text = "";

            }
            //记录申请访问记录
            LogAccess(enteredEmployeeID, machineCode, accessGranted);
        }

        /// <summary>
        /// 验证法生成算法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            string userEnteredCode = textBox5.Text;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
