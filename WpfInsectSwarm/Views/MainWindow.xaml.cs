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
        private Point _lightTarget = new(400, 300); // Mouse target
        private Point _lightAttractor = new(400, 300);
        private Vector _lightVelocity = new(0, 0);
        private double _time = 0;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            SimulationCanvas.MouseMove += (s, e) =>
            {
                _lightTarget = e.GetPosition(SimulationCanvas); // Track where mouse is
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
                    new Vector((_rand.NextDouble() - 0.5) * 6, (_rand.NextDouble() - 0.5) * 6),
                    _rand
                );
                _boids.Add(boid);
            }
        }

        private void Update(object sender, EventArgs e)
        {
            _time += 0.05;
            SimulationCanvas.Children.Clear();

            // Inertia-based light position update
            Vector desired = (Vector)_lightTarget - (Vector)_lightAttractor;
            _lightVelocity += desired * 0.05;
            _lightVelocity *= 0.85;
            _lightAttractor += _lightVelocity;

            for (int i = _boids.Count - 1; i >= 0; i--)
            {
                Boid boid = _boids[i];
                boid.Update(_boids, SimulationCanvas.ActualWidth, SimulationCanvas.ActualHeight, _lightAttractor, _time);

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
        private readonly Random _rand;
        private readonly double personalOffset;
        private double focusLevel;
        private double lostCooldown;
        private bool isLost;
        private Vector wanderTarget;

        public double Direction => Math.Atan2(Velocity.Y, Velocity.X);

        public Boid(Vector position, Vector velocity, Random rand)
        {
            Position = position;
            Velocity = velocity;
            _rand = rand;
            personalOffset = _rand.NextDouble() * 1000;

            focusLevel = rand.NextDouble() < 0.2 ? 0.0 : 1.0;
            lostCooldown = rand.NextDouble() * 3;
            isLost = focusLevel == 0.0;

            wanderTarget = position + new Vector(rand.NextDouble() * 100 - 50, rand.NextDouble() * 100 - 50);
        }

        public void Update(List<Boid> boids, double width, double height, Point light, double time)
        {
            Vector toLight = (Vector)light - Position;
            double distanceToLight = toLight.Length;
            toLight.Normalize();

            // Decide to get lost
            if (!isLost && _rand.NextDouble() < 0.005)
            {
                isLost = true;
                lostCooldown = 2 + _rand.NextDouble() * 4;
                wanderTarget = Position + new Vector(_rand.NextDouble() * 200 - 100, _rand.NextDouble() * 200 - 100);
            }

            // Gradually recover or reset focus
            if (isLost)
            {
                lostCooldown -= 0.05;

                // Bias lost boids toward light if close enough
                if (distanceToLight < 250 && _rand.NextDouble() < 0.05)
                {
                    isLost = false;
                    focusLevel = 0.3;
                }

                // Time-based recovery
                if (lostCooldown <= 0 && _rand.NextDouble() < 0.03)
                {
                    isLost = false;
                    focusLevel = 0.2 + _rand.NextDouble() * 0.5;
                }
            }
            else
            {
                // If far and distracted, reduce focus
                if (distanceToLight > 250 && _rand.NextDouble() < 0.01)
                {
                    focusLevel -= 0.05;
                    if (focusLevel < 0.1)
                    {
                        isLost = true;
                        lostCooldown = 3 + _rand.NextDouble() * 3;
                    }
                }

                // Recover focus over time
                if (focusLevel < 1.0)
                    focusLevel += 0.003;
            }

            focusLevel = Math.Clamp(focusLevel, 0.0, 1.0);

            // Movement vectors
            Vector wanderDir = wanderTarget - Position;
            if (wanderDir.Length > 1)
            {
                wanderDir.Normalize();
            }
            else
            {
                wanderTarget = Position + new Vector(_rand.NextDouble() * 100 - 50, _rand.NextDouble() * 100 - 50);
            }

            double attractionStrength = isLost ? 0.05 : Math.Clamp((distanceToLight / 200) * focusLevel, 0.05, 1.0);
            double wave = Math.Sin(time * 4 + personalOffset) * 0.4;
            Vector perpendicular = new Vector(-toLight.Y, toLight.X) * wave;

            double twitchIntensity = isLost ? 1.5 : 0.3 + (1.0 - focusLevel) * 0.8;
            Vector noise = new Vector((_rand.NextDouble() - 0.5) * twitchIntensity, (_rand.NextDouble() - 0.5) * twitchIntensity);

            Vector acceleration = new Vector(0, 0);

            if (isLost)
            {
                // Mostly wander, small chance to bias toward light
                acceleration = wanderDir * 0.2 + noise;
                if (distanceToLight < 200)
                    acceleration += toLight * 0.05;
            }
            else
            {
                acceleration = toLight * attractionStrength * 0.3 + perpendicular + noise;
            }

            Velocity += acceleration;
            Velocity *= 0.92;

            double maxSpeed = isLost ? 1.2 : 2.5;
            if (Velocity.Length > maxSpeed)
            {
                Velocity.Normalize();
                Velocity *= maxSpeed;
            }

            Position += Velocity;
        }
    }
}
