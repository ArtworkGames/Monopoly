using MessagePipe;
using UnityEngine;
using VContainer;

namespace ArtworkGames.LifetimeScopes
{
	public class LifetimeScopeConfigurator : MonoBehaviour
	{
		public virtual void Configure(IContainerBuilder builder)
		{
		}
		public virtual void ConfigureMessagePipe(IContainerBuilder builder, MessagePipeOptions options)
		{
		}
	}
}
