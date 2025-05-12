using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParallelTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Globalo.motionManager = new MotionControl.MotionManager();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //READY

            Globalo.motionManager.liftMachine.ReadyRun();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //STOP
            Globalo.motionManager.liftMachine.StopAuto();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //PAUSE
            Globalo.motionManager.liftMachine.PauseAuto();
        }
    }
}
