namespace Code.Gameplay.Player.PlayerStateSystem.Base
{
    
    /// Ядро паттерна. Задача машины предельно простая:
    ///   1) хранить ссылку на ТЕКУЩЕЕ состояние (оно всегда ровно одно!),
    ///   2) корректно переключать состояния,
    ///   3) каждый кадр отдавать управление текущему состоянию.
    /// 
    /// Обратите внимание: это обычный C#-класс, а НЕ MonoBehaviour.
    /// Ему не нужны Update, инспектор или объект на сцене —
    /// им управляет PlayerController.
    public class StateMachine
    {
        // Этот элемент синтаксиса C# называется Свойство (Property) — это синтаксический сахар над парой методов:
        // один читает значение, другой записывает. Компилятор C# превращает свойство в методы автоматически.
        // Компилятор на самом деле генерирует примерно следующее:
        // 1. Скрытое приватное поле (backing field). private IState <CurrentState>k__BackingField;
        //    Имя <CurrentState>k__BackingField недопустимо в обычном C# —
        //    так компилятор гарантирует, что вы не обратитесь к полю напрямую.
        // 2. Метод-читатель (виден всем, т.к. свойство public)
        // public IState get_CurrentState()
        // {
        //     return <CurrentState>k__BackingField;
        // }
        // 3. Метод-писатель (виден только внутри класса, т.к. set помечен private)
        // private void set_CurrentState(IState value)
        // {
        //     <CurrentState>k__BackingField = value;
        // }
        //
        // Также, внутри свойств можно располагать дополнительную логику, например
        // private IState currentState; - теперь backing field пишем сами
        // public IState CurrentState
        // {
        //     get => currentState;
        //     private set
        //     {
        //         !!! Дополнительная логика !!!
        //         Debug.Log($"Переход: {currentState?.GetType().Name} -> {value.GetType().Name}");
        //         currentState = value;
        //     }
        // }
        public IState CurrentState { get; private set; }

        public void ChangeState(IState newState)
        {
            // Порядок вызовов важен и является главной гарантией паттерна:
            //   старое.Exit() -> смена ссылки -> новое.Enter()
            // Благодаря этому состояние ВСЕГДА успевает "прибрать за собой",
            // прежде чем начнёт работать следующее.
            
            // ?. — «условное обращение»: если объект не null — обратись к его члену;
            // если null — не делай ничего (а если выражение должно вернуть значение — верни null).
            // CurrentState?.Exit(); - эта конструкция эквивалентна записи:
            // if (CurrentState != null)
            // {
            //     CurrentState.Exit();
            // }
            // А в случае, когда результат используется:
            // string name = CurrentState?.GetType().Name - разворачивается в
            // string name;
            // if (CurrentState != null)
            //     name = CurrentState.GetType().Name;
            // else
            //     name = null;
            //
            // ?. — на самом первом запуске текущего состояния ещё нет,
            // и вызывать Exit() не у кого.
            CurrentState?.Exit();

            CurrentState = newState;
            CurrentState.Enter();
        }

        // Машина не содержит игровой логики — она лишь "передаёт слово"
        // текущему состоянию. Вся логика живёт внутри состояний.
        public void Tick() => CurrentState?.Tick();
    }
}