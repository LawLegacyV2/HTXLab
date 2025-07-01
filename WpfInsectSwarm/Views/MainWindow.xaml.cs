using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Question4_InsectSwarm
{
    public partial class MainWindow : Window
    {
        private readonly List<Boid> _boids = new();
        private readonly Random _rand = new();
        private Point _lightAttractor = new(400, 300);
        private double _time = 0;
        private bool _burst = false;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            SimulationCanvas.MouseMove += (s, e) =>
            {
                Point pos = e.GetPosition(SimulationCanvas);
                _lightAttractor = pos;
            };

            SimulationCanvas.MouseDown += (_, __) =>
            {
                TriggerBurst(30); // Burst 30 insects on click
            };

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };
            _timer.Tick += Update;
            _timer.Start();
        }

        private void TriggerBurst(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var boid = new Boid(
                    new Vector(_rand.NextDouble() * SimulationCanvas.ActualWidth, _rand.NextDouble() * SimulationCanvas.ActualHeight),
                    new Vector((_rand.NextDouble() - 0.5) * 10, (_rand.NextDouble() - 0.5) * 10)
                );
                _boids.Add(boid);
            }
        }

        private void Update(object sender, EventArgs e)
        {
            _time += 0.05;
            SimulationCanvas.Children.Clear();

            for (int i = _boids.Count - 1; i >= 0; i--)
            {
                Boid boid = _boids[i];
                boid.Update(_boids, SimulationCanvas.ActualWidth, SimulationCanvas.ActualHeight, _lightAttractor, _time, _burst);

                Vector p = boid.Position;
                if (p.X < 0 || p.X > SimulationCanvas.ActualWidth ||
                    p.Y < 0 || p.Y > SimulationCanvas.ActualHeight)
                {
                    _boids.RemoveAt(i);
                    continue;
                }

                Polygon triangle = new()
                {
                    Points = new PointCollection
                    {
                        new Point(p.X + 5 * Math.Cos(boid.Direction), p.Y + 5 * Math.Sin(boid.Direction)),
                        new Point(p.X + 5 * Math.Cos(boid.Direction + 2.5), p.Y + 5 * Math.Sin(boid.Direction + 2.5)),
                        new Point(p.X + 5 * Math.Cos(boid.Direction - 2.5), p.Y + 5 * Math.Sin(boid.Direction - 2.5))
                    },
                    Fill = Brushes.LightGreen
                };
                SimulationCanvas.Children.Add(triangle);
            }

            Ellipse light = new()
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Yellow,
                Opacity = 0.4
            };
            Canvas.SetLeft(light, _lightAttractor.X - 10);
            Canvas.SetTop(light, _lightAttractor.Y - 10);
            SimulationCanvas.Children.Add(light);
        }
    }

    public class Boid
    {
        public Vector Position;
        public Vector Velocity;
        public double Direction => Math.Atan2(Velocity.Y, Velocity.X);

        public Boid(Vector position, Vector velocity)
        {
            Position = position;
            Velocity = velocity;
        }

        public void Update(List<Boid> boids, double width, double height, Point light, double time, bool burst)
        {
            Vector acceleration = ((Vector)light - Position);
            acceleration.Normalize();
            acceleration *= 0.5;

            Velocity += acceleration;
            if (Velocity.Length > 4)
            {
                Velocity.Normalize();
                Velocity *= 4;
            }

            Position += Velocity;
        }
    }
}
