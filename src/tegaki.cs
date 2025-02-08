using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace Tegaki
{
    public class Task
    {
        public int Id { get; set; }
        public int PreparationTime { get; set; }
        public int ProcessingTime { get; set; }
        public int RepeatCount { get; set; }
        public int MachineId { get; set; }

        public Task(int id, int preparationTime, int processingTime, int repeatCount, int machineId)
        {
            this.Id = id;
            this.PreparationTime = preparationTime;
            this.ProcessingTime = processingTime;
            this.RepeatCount = repeatCount;
            this.MachineId = machineId;
        }
    }

    public class Machine
    {
        public int Id { get; set;}
        public string Name { get; set; }
        public DateTime MachineStartTime { get; set; }

        public Machine(int id, string name, DateTime machineStartTime)
        {
            this.Id = id;
            this.Name = name;
            this.MachineStartTime = machineStartTime;
        }
    }

    public class Simulator
    {
        public List<Task> Tasks { get; set; }
        public List<Machine> Machines { get; set; }
        public DateTime Start { get; set; }

        public Simulator()
        {
            this.Machines = new List<Machine>();
        }

        public void ReadTasksFromGrid(DataGridView dataGridViewTasks)
        {
            Tasks = new List<Task>();
            foreach (DataGridViewRow row in dataGridViewTasks.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    int Id = int.Parse(row.Cells[0].Value.ToString());
                    int preparationTime = int.Parse(row.Cells[1].Value.ToString());
                    int processingTime = int.Parse(row.Cells[2].Value.ToString());
                    int repeatCount = int.Parse(row.Cells[3].Value.ToString());
                    int machineId = int.Parse(row.Cells[4].Value.ToString());
                    this.Tasks.Add(new Task(Id, preparationTime, processingTime, repeatCount, machineId));
                }
            }
        }

        public void EasyInitMachines(string txtMachineId, DateTime FirstStartTime)
        {
            int machineAmount = int.Parse(txtMachineId);
            Machines = new List<Machine>();
            for (int i = 1; i < machineAmount + 1; i++)
            {
                Machines.Add(new Machine(i, "Machine " + i, FirstStartTime));
            }
            Start = FirstStartTime;
        }

        public void RenderTimeline(Panel panel)
        {
            panel.Controls.Clear();
            int x = 10;
            int y = 100;

            DateTime FirstStartTime = Start;
            DateTime LastFinishedTime = Start;

            foreach (var machine in Machines)
            {
                Label lblMachine = new Label();
                lblMachine.Text = machine.Name;
                lblMachine.Location = new Point(x, y);
                lblMachine.Size = new Size(80, 20);
                panel.Controls.Add(lblMachine);

                x = 100;

                DateTime currentTime = machine.MachineStartTime;
                FirstStartTime = currentTime < FirstStartTime ? currentTime : FirstStartTime;
                foreach (Task task in Tasks)
                {
                    if (task.MachineId == machine.Id)
                    {
                        for (int repeat = 0; repeat < task.RepeatCount; repeat++)
                        {
                            Label lblPreparationTask = new Label();
                            lblPreparationTask.Text = task.Id.ToString();
                            lblPreparationTask.Location = new Point(x, y);
                            lblPreparationTask.Size = new Size(task.PreparationTime, 20);
                            lblPreparationTask.BackColor = Color.Yellow;
                            lblPreparationTask.TextAlign = ContentAlignment.MiddleCenter;
                            panel.Controls.Add(lblPreparationTask);
                            x += task.PreparationTime;
                            
                            Label lblProcessingTask = new Label();
                            lblProcessingTask.Text = task.Id.ToString();
                            lblProcessingTask.Location = new Point(x, y);
                            lblProcessingTask.Size = new Size(task.ProcessingTime, 20);
                            lblProcessingTask.BackColor = Color.Green;
                            lblProcessingTask.TextAlign = ContentAlignment.MiddleCenter;
                            panel.Controls.Add(lblProcessingTask);
                            x += task.ProcessingTime;

                            currentTime = currentTime.AddMinutes(task.PreparationTime + task.ProcessingTime);
                            LastFinishedTime = currentTime > LastFinishedTime ? currentTime : LastFinishedTime;
                        }
                    }
                }
                x = 10;
                y += 30;
            }

            y = 50;
            // Draw Time
            Label lblTime = new Label();
            lblTime.Text = "Timeline";
            lblTime.Location = new Point(x,y);
            panel.Controls.Add(lblTime);
            x = 100;
            y += 20;

            for (int i = 0; i < Math.Ceiling((LastFinishedTime - FirstStartTime).TotalHours); i++)
            {
                Label lblTimeLine = new Label();
                lblTimeLine.Text = FirstStartTime.AddHours(i).ToString("| HH:mm");
                lblTimeLine.Location = new Point(x, y);
                lblTimeLine.Size = new Size(40, 20);
                panel.Controls.Add(lblTimeLine);
                x += 60;
            }
        }

        public void Run(Panel panelTimeline, DataGridView dataGridViewTasks, string txtMachineId, DateTime FirstStartTime)
        {
            ReadTasksFromGrid(dataGridViewTasks);
            EasyInitMachines(txtMachineId, FirstStartTime);
            RenderTimeline(panelTimeline);
        }
    }

    public class MainForm : Form
    {
        private Simulator simulator;
        private TextBox txtPreparationTime;
        private TextBox txtProcessingTime;
        private TextBox txtRepeatCount;
        private TextBox txtMachineId;
        private TextBox startTime;
        private Button btnAddTask;
        private Button btnRunSimulation;
        private DataGridView dataGridViewTasks;
        private Panel panelTimeline;

        public MainForm()
        {
            InitializeComponent();
            simulator = new Simulator();
        }

        public void InitializeComponent() {
            this.txtPreparationTime = new TextBox();
            this.txtProcessingTime = new TextBox();
            this.txtRepeatCount = new TextBox();
            this.txtMachineId = new TextBox();
            this.startTime = new TextBox();
            this.btnAddTask = new Button();
            this.btnRunSimulation = new Button();
            this.dataGridViewTasks = new DataGridView();
            this.panelTimeline = new Panel();
            
            // txtPreparationTime
            this.txtPreparationTime.Location = new Point(10, 50);
            this.txtPreparationTime.Size = new Size(100, 20);
            this.txtPreparationTime.Text = "10";

            // txtProcessingTime
            this.txtProcessingTime.Location = new Point(120, 50);
            this.txtProcessingTime.Size = new Size(100, 20);
            this.txtProcessingTime.Text = "20";

            // txtRepeatCount
            this.txtRepeatCount.Location = new Point(230, 50);
            this.txtRepeatCount.Size = new Size(100, 20);
            this.txtRepeatCount.Text = "1";

            // txtMachineId
            this.txtMachineId.Location = new Point(340, 50);
            this.txtMachineId.Size = new Size(100, 20);
            this.txtMachineId.Text = "1";

            // startTime
            this.startTime.Location = new Point(600, 10);
            this.startTime.Size = new Size(100, 20);
            this.startTime.Text = "08:30:00";

            // btnAddTask
            this.btnAddTask.Location = new Point(450, 50);
            this.btnAddTask.Size = new Size(100, 20);
            this.btnAddTask.Text = "Add Task";
            this.btnAddTask.Click += new EventHandler(this.OnAddTask_Click);

            // btnRunSimulation
            this.btnRunSimulation.Location = new Point(570, 50);
            this.btnRunSimulation.Size = new Size(100, 20);
            this.btnRunSimulation.Text = "Run Simulation";
            this.btnRunSimulation.Click += new EventHandler(this.OnRunSimulation_Click);

            // dataGridViewTasks
            this.dataGridViewTasks.Location = new Point(10, 80);
            this.dataGridViewTasks.Size = new Size(780, 200);
            this.dataGridViewTasks.Columns.Add("Id", "ID");
            this.dataGridViewTasks.Columns.Add("PreparationTime", "Preparation Time");
            this.dataGridViewTasks.Columns.Add("ProcessingTime", "Processing Time");
            this.dataGridViewTasks.Columns.Add("RepeatCount", "Repeat Count");
            this.dataGridViewTasks.Columns.Add("MachineId", "Machine Id");
            this.dataGridViewTasks.CellValueChanged += new DataGridViewCellEventHandler(this.OnCellValueChanged);

            // panelTimeline
            this.panelTimeline.Location = new Point(10, 260);
            this.panelTimeline.Size = new Size(780, 300);
            this.panelTimeline.AutoScroll = true;

            // Main Form
            this.Controls.Add(this.txtPreparationTime);
            this.Controls.Add(this.txtProcessingTime);
            this.Controls.Add(this.txtRepeatCount);
            this.Controls.Add(this.txtMachineId);
            this.Controls.Add(this.startTime);
            this.Controls.Add(this.btnAddTask);
            this.Controls.Add(this.btnRunSimulation);
            this.Controls.Add(this.dataGridViewTasks);
            this.Controls.Add(this.panelTimeline);

            this.Text = "shumiraton";
            this.Size = new Size(800, 600);
            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
        }

        private void OnAddTask_Click(object sender, EventArgs e)
        {
            int taskId = this.dataGridViewTasks.Rows.Count;
            int preparationTime = int.Parse(this.txtPreparationTime.Text);
            int processingTime = int.Parse(this.txtProcessingTime.Text);
            int repeatCount = int.Parse(this.txtRepeatCount.Text);
            int machineId = int.Parse(this.txtMachineId.Text);

            this.dataGridViewTasks.Rows.Add(taskId, preparationTime, processingTime, repeatCount, machineId);
        }

        private void OnRunSimulation_Click(object sender, EventArgs e)
        {
            DateTime start;
            if(!DateTime.TryParse(this.startTime.Text, out start))
            {
                MessageBox.Show("Invalid Start Time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            simulator.Run(panelTimeline, dataGridViewTasks, txtMachineId.Text, start);
        }

        private void OnCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            /*
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell cell = this.dataGridViewTasks.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell.Value != null)
                {
                    Console.WriteLine("Cell Value Changed: " + cell.Value.ToString());
                }
            }
            */
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}