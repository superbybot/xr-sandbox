using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using App.Demos.RacingDemo.Scripts.Domain;
using App.Demos.RacingDemo.Scripts.Domain.UseCases;
using App.Demos.RacingDemo.Scripts.Presentation.Views;

namespace App.Demos.RacingDemo.Scripts
{
    public class RacingLifetimeScope : LifetimeScope
    {
        [SerializeField] private int totalLaps = 3;

        protected override void Configure(IContainerBuilder builder)
        {
            // 1. Domain (Entities)
            // Register RacingState as a specific instance or using a factory if complex
            builder.Register<RacingState>(Lifetime.Singleton)
                .WithParameter("totalLaps", totalLaps);

            // 2. Domain (Use Cases)
            // Register Interfaces to Concrete implementations
            builder.Register<StartRaceUseCase>(Lifetime.Singleton).As<IStartRaceUseCase>();
            builder.Register<ProcessCheckpointUseCase>(Lifetime.Singleton).As<IProcessCheckpointUseCase>();

            // 3. Presentation (Views)
            // Inject into existing scene components
            // This finds the component in the scene and registers it, AND injects into it.
            builder.RegisterComponentInHierarchy<RacingView>();

            // For LapCheckpoints, there are many. 
            // We can rely on VContainer's "Auto Inject Game Objects" feature on the LifetimeScope inspector.
            // But to be explicit, we could find them. 
            // However, VContainer usually recommends AutoInject for broad scene usage.
        }
    }
}
