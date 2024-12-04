using System;

namespace ShineCore
{
    public class Promise
    {
        public Action completedCallBack;
        public bool isCompleted = false;
        public Promise(Action<Action> action)
        {
            action.Invoke(Resolve);
        }

        public Promise()
        {
            isCompleted = true;
        }

        public void Then(Action action)
        {
            completedCallBack += action;
            if (isCompleted)
            {
                completedCallBack.Invoke();
                completedCallBack = null;
            }
        }

        public void Resolve()
        {
            isCompleted = true;
            completedCallBack?.Invoke();
            completedCallBack = null;
        }

        public static Promise All(Promise[] promises)
        {
            return new Promise(resolve =>
            {
                int count = 0;
                int promisesLength = promises.Length;
                foreach (Promise promise in promises)
                {
                    promise.Then(() =>
                    {
                        count++;
                        if (count == promisesLength)
                        {
                            resolve();
                        }
                    });
                }
            });
        }
    }
}