﻿using System;

namespace QFramework
{
    public class FrameIntervalObservable<T> : OperatorObservableBase<QFramework.FrameInterval<T>>
    {
        readonly IObservable<T> source;

        public FrameIntervalObservable(IObservable<T> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<QFramework.FrameInterval<T>> observer, IDisposable cancel)
        {
            return source.Subscribe(new FrameInterval(observer, cancel));
        }

        class FrameInterval : OperatorObserverBase<T, QFramework.FrameInterval<T>>
        {
            int lastFrame;

            public FrameInterval(IObserver<QFramework.FrameInterval<T>> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.lastFrame = UnityEngine.Time.frameCount;
            }

            public override void OnNext(T value)
            {
                var now = UnityEngine.Time.frameCount;
                var span = now - lastFrame;
                lastFrame = now;

                base.observer.OnNext(new QFramework.FrameInterval<T>(value, span));
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }
}