using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CamWidget;
using System.Threading;
using RCComm;
using DES_Sharp;
using logisteering;
using System.Net.NetworkInformation;
using System.IO;
using basic_remote_truck;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

namespace basic_4cam
{

    public partial class Form1 : Form
    {
        //调用摄像头
        CamMngr mngr = new CamMngr();

        //调用socket结构体
        DGramRcver<UplinkDGram> rcver = new DGramRcver<UplinkDGram>();
        DgramSender<DownlinkDGram> sd = new DgramSender<DownlinkDGram>();
        UplinkDGram obj = new UplinkDGram();
        DownlinkDGram data = new DownlinkDGram();

        DGramRcver<UplinkDGram> rcver2 = new DGramRcver<UplinkDGram>();
        DgramSender<DownlinkDGram> sd2 = new DgramSender<DownlinkDGram>();
        UplinkDGram obj2 = new UplinkDGram();

        DGramRcver<UplinkDGram> rcver3 = new DGramRcver<UplinkDGram>();
        DgramSender<DownlinkDGram> sd3 = new DgramSender<DownlinkDGram>();
        UplinkDGram obj3 = new UplinkDGram();

        DGramRcver<radar> rd_socket = new DGramRcver<radar>();
        radar rd = new radar();

        //调用方向盘的类
        logi steer = new logi();

        //创建锁
        public static object LockData = new object();

        public Form1()
        {
            InitializeComponent();

            //初始化相机并调用
            CamMngr.Init();
            mngr.FromXml("../bin/config/4cam.xml");
            mngr.DoLoginAll();
            Thread.Sleep(100);
            mngr.DoPlayAll();
            camPlayerWnd1.Visible = true;
            camPlayerWnd2.Visible = true;
            camPlayerWnd3.Visible = true;
            camPlayerWnd4.Visible = true;
            camPlayerWnd5.Visible = false;
            camPlayerWnd6.Visible = false;
            camPlayerWnd7.Visible = false;
            camPlayerWnd8.Visible = false;
            camPlayerWnd9.Visible = false;
            camPlayerWnd10.Visible = false;
            camPlayerWnd11.Visible = false;
            camPlayerWnd12.Visible = false;


            //启动三辆车Rcver线程
            recv_car_info(1);
            recv_car_info(2);
            recv_car_info(3);

            //获取车名
            get_car_name(1);
            get_car_name(2);
            get_car_name(3);

            //初始化方向盘并更新
            steer.init();
            steer.update();

            //雷达
            rd_socket.Start(8866);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chartsetting();
            draw();
            socket.Tick += new EventHandler(socket_Tick);
            socket.Start();
            change.Tick += new EventHandler(change_Tick);
            change.Start();
            nettime.Tick += new EventHandler(nettime_Tick);
            nettime.Start();
            radar.Tick += new EventHandler(radar_Tick);
            radar.Start();
        }

        string ip_addr_recv;
        int port;
        int port_rec;
        string car_name;
        private void get_car_name(int car_id)
        {
            XmlDocument doc_car = new XmlDocument();
            doc_car.Load(@"../bin/config/carname.xml");
            XmlNode xn = doc_car.SelectSingleNode("cars");
            XmlNodeList xnl = xn.ChildNodes;

            foreach (XmlNode xn1 in xnl)
            {
                XmlElement xe = (XmlElement)xn1;
                int car = 0;
                bool id_juage = int.TryParse(xe.GetAttribute("id"), out car);
                if (car == car_id)
                {
                    car_name = xe.GetAttribute("name");
                    if (car == 1)
                    {
                        label63.Text = car_name;
                    }
                    if (car == 2)
                    {
                        label64.Text = car_name;
                    }
                    if (car == 3)
                    {
                        label65.Text = car_name;
                    }
                }
            }
        }
        private void recv_car_info(int car_id)
        {
            XmlDocument doc_car = new XmlDocument();
            doc_car.Load(@"../bin/config/carname.xml");
            XmlNode xn = doc_car.SelectSingleNode("cars");
            XmlNodeList xnl = xn.ChildNodes;
            
            bool  port_rec_ok;
            foreach (XmlNode xn1 in xnl)
            {
                XmlElement xe = (XmlElement)xn1;
                int car = 0;
                bool id_juage = int.TryParse(xe.GetAttribute("id"), out car);
                if (car == car_id)
                {
                    port_rec_ok = int.TryParse(xe.GetAttribute("port_rec"), out port_rec);
                    if (car==1)
                    {
                        rcver.Start(port_rec);
                    }
                    if (car == 2)
                    {
                        rcver2.Start(port_rec);
                    }
                    if (car == 3)
                    {
                        rcver3.Start(port_rec);
                    }
                }
            }
        }

        private void send_car_info(int car_id)
        {
            XmlDocument doc_car = new XmlDocument();
            doc_car.Load(@"../bin/config/carname.xml");
            XmlNode xn = doc_car.SelectSingleNode("cars");
            XmlNodeList xnl = xn.ChildNodes;

            bool port_ok;
            foreach (XmlNode xn1 in xnl)
            {
                XmlElement xe = (XmlElement)xn1;
                int car = 0;
                bool id_juage = int.TryParse(xe.GetAttribute("id"), out car);
                if (car == car_id)
                {
                    ip_addr_recv = xe.GetAttribute("ip");
                    port_ok = int.TryParse(xe.GetAttribute("port"), out port);
                    if (car == 1)
                    {
                        sd.Start(ip_addr_recv, port);
                    }
                    if (car == 2)
                    {
                        sd2.Start(ip_addr_recv, port);
                    }
                    if (car == 3)
                    {
                        sd3.Start(ip_addr_recv, port);
                    }
                }
            }
        }

        string get_ip;
        private void get_ipstr(int car_id)
        {
            XmlDocument doc_car = new XmlDocument();
            doc_car.Load(@"../bin/config/carname.xml");
            XmlNode xn = doc_car.SelectSingleNode("cars");
            XmlNodeList xnl = xn.ChildNodes;
            foreach (XmlNode xn1 in xnl)
            {
                XmlElement xe = (XmlElement)xn1;
                int car = 0;
                bool id_juage = int.TryParse(xe.GetAttribute("id"), out car);
                if (car == car_id)
                {
                    get_ip = xe.GetAttribute("ip");
                }
            }
        }


        Ping pingSender = new Ping();
        string[] ipstr = new string[3];
        short suggest_speed;
        string transdata = "a";
        int timeout = 1000;
        private void nettime_Tick(object sender, EventArgs e)
        {
            for (int i=1;i<=3;i++)
            {
                get_ipstr(i);
                ipstr[i - 1] = get_ip;
            }
            byte[] buffer = Encoding.ASCII.GetBytes(transdata);

            //Ping 选项设置  
            PingOptions options1 = new PingOptions();
            PingOptions options2 = new PingOptions();
            PingOptions options3 = new PingOptions();
            options1.DontFragment = true;
            options2.DontFragment = true;
            options3.DontFragment = true;

            PingReply reply1 = pingSender.Send(ipstr[0], timeout, buffer, options1);
            PingReply reply2 = pingSender.Send(ipstr[1], timeout, buffer, options2);
            PingReply reply3 = pingSender.Send(ipstr[2], timeout, buffer, options3);

            if (reply1.Status == IPStatus.Success)
            {
                label55.Text = "连接成功";
                label56.Text = reply1.RoundtripTime.ToString() + "ms";

            }
            else if (reply1.Status == IPStatus.TimedOut)
            {
                label55.Text = "连接超时";
            }
            else
            {
                label55.Text = "连接失败";
            }
            label57.Text = DateTime.Now.ToLongTimeString();
            label58.Text = DateTime.Now.ToLongDateString();
            
            long range1 = reply1.RoundtripTime;
            long range2 = reply2.RoundtripTime;
            long range3 = reply3.RoundtripTime;

            Series NET = chart1.Series[0];
            Series NET2 = chart1.Series[1];
            Series NET3 = chart1.Series[2];
            NET.Name = label63.Text;
            NET2.Name = label64.Text;
            NET3.Name = label65.Text;

            DateTime time = DateTime.Now;
            NET.Points.AddXY(time, range1+1); 
            NET2.Points.AddXY(time, range2+1); 
            NET3.Points.AddXY(time, range3+1);

            chart1.ChartAreas[0].AxisX.ScaleView.Position = NET.Points.Count - 5;

            suggest_speed = (short)(20 - range1 * 0.055);
            label59.Text = suggest_speed.ToString() + "km/h";
        }
        private void chartsetting()
        {
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss";
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 3;
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            
        }

        private void socket_Tick(object sender, EventArgs e)
        {
            lock (LockData)
            {
                // FetchData为线程外异步获取数据的接口
                rcver.FetchData(ref obj);

                label10.Text = obj.m_vehicle_state.ToString();
                label11.Text = obj.m_speed.ToString();
                label12.Text = obj.m_rpm.ToString();
                
                
                label15.Text = obj.m_steering.ToString();
                label16.Text = ((int)obj.m_path_heading).ToString();
                label17.Text = ((double)obj.m_path_x / 1000000).ToString();
                label18.Text = ((double)obj.m_path_y / 1000000).ToString();

               
                // FetchData为线程外异步获取数据的接口
                rcver2.FetchData(ref obj2);

                label28.Text = obj2.m_vehicle_state.ToString();
                label29.Text = obj2.m_speed.ToString();
                label30.Text = obj2.m_rpm.ToString();

                label33.Text = obj2.m_steering.ToString();
                label34.Text = obj2.m_path_heading.ToString();
                label35.Text = ((double)obj2.m_path_x / 1000000).ToString();
                label36.Text = ((double)obj2.m_path_y / 1000000).ToString();

                // FetchData为线程外异步获取数据的接口
                rcver3.FetchData(ref obj3);

                label46.Text = obj3.m_vehicle_state.ToString();
                label47.Text = obj3.m_speed.ToString();
                label48.Text = obj3.m_rpm.ToString();
                
                label51.Text = obj3.m_steering.ToString();
                label52.Text = obj3.m_path_heading.ToString();
                label53.Text = ((double)obj3.m_path_x / 1000000).ToString();
                label54.Text = ((double)obj3.m_path_y / 1000000).ToString();
                
            }
        }

        private void radar_Tick(object sender, EventArgs e)
        {
            rd_socket.FetchData(ref rd);

            if (rd.front1 >0&&rd.front1 <= 1000)
            {
                f1.Visible = false;
                f2.Visible = false;
                f3.Visible = true;
            }

            int mid = (rd.front2 < rd.front3 ? rd.front2 : rd.front3);
            if (mid > 1000 && mid < 2000)
            {
                f4.Visible = false;
                f5.Visible = true;
                f6.Visible = false;
            }

            if (rd.front4 >= 2000 && rd.front4 < 4000)
            {
                f7.Visible = true;
                f8.Visible = false;
                f9.Visible = false;
            }

            if (rd.rear1 >0&&rd.rear1 <= 1000)
            {
                r1.Visible = true;
                r2.Visible = false;
                r3.Visible = false;
            }

            int midr = (rd.rear2 < rd.rear3 ? rd.rear2 : rd.rear3);
            if (midr > 1000 && midr < 2000)
            {
                r1.Visible = false;
                r2.Visible = true;
                r3.Visible = false;
            }

            if (rd.rear4 >= 2000 && rd.rear4 < 4000)
            {
                r1.Visible = false;
                r2.Visible = false;
                r3.Visible = true;
            }

        }

        public delegate void DataCommunicationHandler_choose(int choice);
        public event DataCommunicationHandler_choose choose;
        private bool Flag_change()
        {
            if(label13.Text == " "&&label14.Text == " "&&label49.Text == " "&&label50.Text == " "&&label31.Text == " "&&label32.Text == " ")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void change_Tick(object sender, EventArgs e)
        {
            if (Flag_change() == true)
            {
                if (steer.escape() == true)
                {
                    System.Environment.Exit(0);
                }

                if (changebox1.Visible == true && changebox2.Visible == false && changebox3.Visible == false)//如果位置在1
                {
                    if (steer.switch_car() == 4)
                    {
                        changebox1.Visible = false;
                        changebox2.Visible = true;
                        changebox3.Visible = false;
                    }
                    if (steer.switch_car() == 2)
                    {
                        changebox1.Visible = false;
                        changebox2.Visible = false;
                        changebox3.Visible = true;
                    }
                    if (steer.cam_monitor() == true)
                    {
                        cam1_open();
                    }
                    if (steer.take_control() == true)
                    {
                        sd2.Stop();
                        sd3.Stop();
                        send_car_info(1);//启动1号车控制发送
                        control2.Stop();
                        control3.Stop();
                        Thread.Sleep(20);
                        control1.Tick += new EventHandler(control1_Tick);
                        control1.Start();

                        choose(1);
                    }
                    if (steer.gps_loc() == true)
                    {
                        map2.Stop();
                        map3.Stop();
                        Thread.Sleep(20);
                        map1.Tick += new EventHandler(map1_Tick);
                        map1.Start();
                    }
                    Thread.Sleep(10);
                }
                else if (changebox1.Visible == false && changebox2.Visible == true && changebox3.Visible == false)//如果位置在2
                {
                    if (steer.switch_car() == 4)
                    {
                        changebox1.Visible = false;
                        changebox2.Visible = false;
                        changebox3.Visible = true;
                    }
                    if (steer.switch_car() == 2)
                    {
                        changebox1.Visible = true;
                        changebox2.Visible = false;
                        changebox3.Visible = false;
                    }
                    if (steer.cam_monitor() == true)
                    {
                        cam2_open();
                    }
                    if (steer.take_control() == true)
                    {
                        sd.Stop();
                        sd3.Stop();
                        send_car_info(2);//启动2号车控制发送
                        control1.Stop();
                        control3.Stop();
                        Thread.Sleep(20);
                        control2.Tick += new EventHandler(control2_Tick);
                        control2.Start();

                        choose(2);
                    }
                    if (steer.gps_loc() == true)
                    {
                        map1.Stop();
                        map3.Stop();
                        Thread.Sleep(20);
                        map2.Tick += new EventHandler(map1_Tick);
                        map2.Start();
                    }
                    Thread.Sleep(10);
                }
                else if (changebox1.Visible == false && changebox2.Visible == false && changebox3.Visible == true)//如果位置在3
                {
                    if (steer.switch_car() == 4)
                    {
                        changebox1.Visible = true;
                        changebox2.Visible = false;
                        changebox3.Visible = false;
                    }
                    if (steer.switch_car() == 2)
                    {
                        changebox1.Visible = false;
                        changebox2.Visible = true;
                        changebox3.Visible = false;
                    }
                    if (steer.cam_monitor() == true)
                    {
                        cam3_open();
                    }
                    if (steer.take_control() == true)
                    {
                        sd2.Stop();
                        sd.Stop();
                        send_car_info(3);//启动3号车控制发送
                        control1.Stop();
                        control2.Stop();
                        Thread.Sleep(20);
                        control3.Tick += new EventHandler(control3_Tick);
                        control3.Start();

                        choose(3);

                    }
                    if (steer.gps_loc() == true)
                    {
                        map2.Stop();
                        map1.Stop();
                        Thread.Sleep(20);
                        map3.Tick += new EventHandler(map1_Tick);
                        map3.Start();
                    }
                    Thread.Sleep(10);
                }
            }       
        }
        public delegate void DataCommunicationHandler(short rpm,int speed,short geer);
        public event DataCommunicationHandler ReciveData;
        private void control1_Tick(object sender, EventArgs e)
        {
            steer.update();
            label13.Text = ((short)steer.SteeringWheelAngle()).ToString();
            label14.Text = steer.accelerator_range().ToString();
            data.m_rc_steer = (int)steer.SteeringWheelAngle()*100;
            data.m_rc_acc_pedal = (short)steer.accelerator_range();
            data.m_rc_brake_pedal = (short)steer.brake_range();
            data.m_rc_gear = (short)steer.car_gear();
            data.m_ctrl_sign = 1;

            sd.UpdateData(data);

            ReciveData(obj.m_rpm, obj.m_speed, data.m_rc_gear);//触发事件
            
            if (steer.exchange() == true)
            {
                data.m_rc_steer = 0;
                data.m_rc_acc_pedal = 0;
                data.m_rc_brake_pedal = 0;
                data.m_rc_gear = 3;
                data.m_ctrl_sign = 0;
                sd.UpdateData(data);
                control1.Stop();
                control2.Stop();
                control3.Stop();
                map1.Stop();
                map2.Stop();
                map3.Stop();
                label13.Text = " ";
                label14.Text = " ";
                choose(0);
            }
            label31.Text = " ";
            label32.Text = " ";
            label49.Text = " ";
            label50.Text = " ";
        }

        private void control2_Tick(object sender, EventArgs e)
        {
            steer.update();

            label31.Text = steer.SteeringWheelAngle().ToString();
            label32.Text = steer.accelerator_range().ToString();
            data.m_rc_steer = (int)steer.SteeringWheelAngle() * 100;
            data.m_rc_acc_pedal = (short)steer.accelerator_range();
            data.m_rc_brake_pedal = (short)steer.brake_range();
            data.m_rc_gear = (short)steer.car_gear();
            data.m_ctrl_sign = 1;

            sd2.UpdateData(data);

            ReciveData(obj2.m_rpm, obj2.m_speed, data.m_rc_gear);//触发事件
            
            if (steer.exchange() == true)
            {
                data.m_rc_steer = 0;
                data.m_rc_acc_pedal = 0;
                data.m_rc_brake_pedal = 0;
                data.m_rc_gear = 3;
                data.m_ctrl_sign = 0;
                sd2.UpdateData(data);
                control1.Stop();
                control2.Stop();
                control3.Stop();
                map1.Stop();
                map2.Stop();
                map3.Stop();
                label31.Text = " ";
                label32.Text = " ";

                choose(0);
            }
            label13.Text = " ";
            label14.Text = " ";
            label49.Text = " ";
            label50.Text = " ";
        }

        private void control3_Tick(object sender, EventArgs e)
        {
            steer.update();

            label49.Text = steer.SteeringWheelAngle().ToString();
            label50.Text = steer.accelerator_range().ToString();
            data.m_rc_steer = (int)steer.SteeringWheelAngle() * 100;
            data.m_rc_acc_pedal = (short)steer.accelerator_range();
            data.m_rc_brake_pedal = (short)steer.brake_range();
            data.m_rc_gear = (short)steer.car_gear();
            data.m_ctrl_sign = 1;

            sd3.UpdateData(data);

            ReciveData(obj3.m_rpm, obj3.m_speed, data.m_rc_gear);//触发事件

            if (steer.exchange() == true)
            {
                data.m_rc_steer = 0;
                data.m_rc_acc_pedal = 0;
                data.m_rc_brake_pedal = 0;
                data.m_rc_gear = 3;
                data.m_ctrl_sign = 0;
                sd3.UpdateData(data);
                control1.Stop();
                control2.Stop();
                control3.Stop();
                map1.Stop();
                map2.Stop();
                map3.Stop();
                label49.Text = " ";
                label50.Text = " ";

                choose(0);
            }
            label13.Text = " ";
            label14.Text = " ";
            label31.Text = " ";
            label32.Text = " ";
        }

        public delegate void DataCommunicationHandler_map(double x,double y);
        public event DataCommunicationHandler_map map_loc;
        private void map1_Tick(object sender, EventArgs e)
        {
            map_loc((double)obj.m_path_x / 1000000, (double)obj.m_path_y / 1000000);

            if (steer.exchange() == true)
            {
                map1.Stop();
                map2.Stop();
                map3.Stop();
            }
        }

        private void map2_Tick(object sender, EventArgs e)
        {
            //电子地图通信
            map_loc((double)obj2.m_path_x / 1000000, (double)obj2.m_path_y / 1000000);

            if (steer.exchange() == true)
            {
                map1.Stop();
                map2.Stop();
                map3.Stop();
            }
        }

        private void map3_Tick(object sender, EventArgs e)
        {
            //电子地图通信
            map_loc((double)obj3.m_path_x / 1000000, (double)obj3.m_path_y / 1000000);

            if (steer.exchange() == true)
            {
                map1.Stop();
                map2.Stop();
                map3.Stop();
            }
        }
        private void cam1_open()
        {
            camPlayerWnd1.Visible = true;
            camPlayerWnd2.Visible = true;
            camPlayerWnd3.Visible = true;
            camPlayerWnd4.Visible = true;
            camPlayerWnd5.Visible = false;
            camPlayerWnd6.Visible = false;
            camPlayerWnd7.Visible = false;
            camPlayerWnd8.Visible = false;
            camPlayerWnd9.Visible = false;
            camPlayerWnd10.Visible = false;
            camPlayerWnd11.Visible = false;
            camPlayerWnd12.Visible = false;
        }

        private void cam2_open()
        {
            camPlayerWnd1.Visible = false;
            camPlayerWnd2.Visible = false;
            camPlayerWnd3.Visible = false;
            camPlayerWnd4.Visible = false;
            camPlayerWnd5.Visible = true;
            camPlayerWnd6.Visible = true;
            camPlayerWnd7.Visible = true;
            camPlayerWnd8.Visible = true;
            camPlayerWnd9.Visible = false;
            camPlayerWnd10.Visible = false;
            camPlayerWnd11.Visible = false;
            camPlayerWnd12.Visible = false;
        }

        private void cam3_open()
        {
            camPlayerWnd1.Visible = false;
            camPlayerWnd2.Visible = false;
            camPlayerWnd3.Visible = false;
            camPlayerWnd4.Visible = false;
            camPlayerWnd5.Visible = false;
            camPlayerWnd6.Visible = false;
            camPlayerWnd7.Visible = false;
            camPlayerWnd8.Visible = false;
            camPlayerWnd9.Visible = true;
            camPlayerWnd10.Visible = true;
            camPlayerWnd11.Visible = true;
            camPlayerWnd12.Visible = true;
        }
        private void draw()
        {
            logobox.BackgroundImage = Image.FromFile("../bin/img/logo.jpg");
            pictureBox4.BackgroundImage = Image.FromFile("../bin/img/car1.jpg");
            pictureBox5.BackgroundImage = Image.FromFile("../bin/img/car2.jpg");
            pictureBox6.BackgroundImage = Image.FromFile("../bin/img/car3.jpg");
            pictureBox4.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox5.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox6.BackgroundImageLayout = ImageLayout.Stretch;


            pictureBox4.Parent = picbox1;
            label1.Parent = picbox1;
            label2.Parent = picbox1;
            label3.Parent = picbox1;
            label4.Parent = picbox1;
            label5.Parent = picbox1;
            label6.Parent = picbox1;
            label7.Parent = picbox1;
            label8.Parent = picbox1;
            label9.Parent = picbox1;
            label10.Parent = picbox1;
            label11.Parent = picbox1;
            label12.Parent = picbox1;
            label13.Parent = picbox1;
            label14.Parent = picbox1;
            label15.Parent = picbox1;
            label16.Parent = picbox1;
            label17.Parent = picbox1;
            label18.Parent = picbox1;

            pictureBox5.Parent = picbox1;
            label19.Parent = picbox1;
            label20.Parent = picbox1;
            label21.Parent = picbox1;
            label22.Parent = picbox1;
            label23.Parent = picbox1;
            label24.Parent = picbox1;
            label25.Parent = picbox1;
            label26.Parent = picbox1;
            label27.Parent = picbox1;
            label28.Parent = picbox1;
            label29.Parent = picbox1;
            label30.Parent = picbox1;
            label31.Parent = picbox1;
            label32.Parent = picbox1;
            label33.Parent = picbox1;
            label34.Parent = picbox1;
            label35.Parent = picbox1;
            label36.Parent = picbox1;

            pictureBox6.Parent = picbox1;
            label37.Parent = picbox1;
            label38.Parent = picbox1;
            label39.Parent = picbox1;
            label40.Parent = picbox1;
            label41.Parent = picbox1;
            label42.Parent = picbox1;
            label43.Parent = picbox1;
            label44.Parent = picbox1;
            label45.Parent = picbox1;
            label46.Parent = picbox1;
            label47.Parent = picbox1;
            label48.Parent = picbox1;
            label49.Parent = picbox1;
            label50.Parent = picbox1;
            label51.Parent = picbox1;
            label52.Parent = picbox1;
            label53.Parent = picbox1;
            label54.Parent = picbox1;
            label55.Parent = picbox1;
            label56.Parent = picbox1;
            label57.Parent = picbox1;
            label58.Parent = picbox1;
            label59.Parent = picbox1;
            label60.Parent = picbox1;
            label61.Parent = picbox1;
            label62.Parent = picbox1;
            label63.Parent = picbox1;
            label64.Parent = picbox1;
            label65.Parent = picbox1;


            chart1.Parent = picbox1;


            changebox1.Parent = picbox1;
            changebox2.Parent = picbox1;
            changebox3.Parent = picbox1;
            changebox1.Visible = true;
            changebox2.Visible = false;
            changebox3.Visible = false;

            label13.Text = " ";
            label14.Text = " ";
            label49.Text = " ";
            label50.Text = " ";
            label31.Text = " ";
            label32.Text = " ";

            f1.Visible = false;
            f2.Visible = false;
            f3.Visible = false;
            f4.Visible = false;
            f5.Visible = false;
            f6.Visible = false;
            f7.Visible = false;
            f8.Visible = false;
            f9.Visible = false;
            r1.Visible = false;
            r2.Visible = false;
            r3.Visible = false;
            r4.Visible = false;
            r5.Visible = false;
            r6.Visible = false;
            r7.Visible = false;
            r8.Visible = false;
            r9.Visible = false;

            f1.Parent = picbox1;
            f2.Parent = picbox1;
            f3.Parent = picbox1;
            f4.Parent = picbox1;
            f5.Parent = picbox1;
            f6.Parent = picbox1;
            f7.Parent = picbox1;
            f8.Parent = picbox1;
            f9.Parent = picbox1;
            r1.Parent = picbox1;
            r2.Parent = picbox1;
            r3.Parent = picbox1;
            r4.Parent = picbox1;
            r5.Parent = picbox1;
            r6.Parent = picbox1;
            r7.Parent = picbox1;
            r8.Parent = picbox1;
            r9.Parent = picbox1;


        }

        
    }
}
