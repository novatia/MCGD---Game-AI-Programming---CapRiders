using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;
using System.Collections.Generic;

using WiFiInput.Server;

public sealed class InputModule : PointerInputModule
{
    [SerializeField]
    private List<string> m_DefaultPlayers = new List<string>();
    [SerializeField]
    private List<string> m_DefaultWifiPlayers = new List<string>();

    private List<PlayerInput> m_Players = new List<PlayerInput>();
    private List<WiFiPlayerInput> m_WifiPlayers = new List<WiFiPlayerInput>();

    [SerializeField]
    private string m_HorizontalAxis = "Horizontal";
    [SerializeField]
    private string m_VerticalAxis = "Vertical";
    [SerializeField]
    private string m_SubmitButton = "Submit";
    [SerializeField]
    private string m_CancelButton = "Cancel";

    [SerializeField]
    private string m_WifiHorizontalAxis = "Horizontal";
    [SerializeField]
    private string m_WifiVerticalAxis = "Vertical";
    [SerializeField]
    private string m_WifiSubmitButton = "Submit";
    [SerializeField]
    private string m_WifiCancelButton = "Cancel";

    [SerializeField]
    private float m_InputActionsPerSecond = 4.5f;
    [SerializeField]
    private bool m_MoveOneElementPerAxisPress = false;
    [SerializeField]
    private float m_RepeatDelay = 0.0f;
    [SerializeField]
    private float m_AxisDeadzone = 0.2f;

    [SerializeField]
    private bool m_AllowMouseInput = false;
    [SerializeField]
    private bool m_AllowMouseInputIfTouchSupported = false;

    [SerializeField]
    private bool m_AllowActivationOnMobileDevice = false;

    private int m_ConsecutiveMoveCount = 0;
    private float m_PrevActionTime = 0f;
    private Vector2 m_LastMoveVector = Vector2.zero;

    private Vector2 m_LastMousePosition = Vector2.zero;
    private Vector2 m_MousePosition = Vector2.zero;

    private bool m_IsTouchSupported = false;

    public event Action playersChangedEvent = null;

    // ACCESSORS

    public string horizontalAxis
    {
        get { return m_HorizontalAxis; }
        set { m_HorizontalAxis = value; }
    }

    public string verticalAxis
    {
        get { return m_VerticalAxis; }
        set { m_VerticalAxis = value; }
    }

    public string submitButton
    {
        get { return m_SubmitButton; }
        set { m_SubmitButton = value; }
    }

    public string cancelButton
    {
        get { return m_CancelButton; }
        set { m_CancelButton = value; }
    }

    public string wifiHorizontalAxis
    {
        get { return m_WifiHorizontalAxis; }
        set { m_WifiHorizontalAxis = value; }
    }

    public string wifiVerticalAxis
    {
        get { return m_WifiVerticalAxis; }
        set { m_WifiVerticalAxis = value; }
    }

    public string wifiSubmitButton
    {
        get { return m_WifiSubmitButton; }
        set { m_WifiSubmitButton = value; }
    }

    public string wifiCancelButton
    {
        get { return m_WifiCancelButton; }
        set { m_WifiCancelButton = value; }
    }

    public float inputActionsPerSecond
    {
        get { return m_InputActionsPerSecond; }
        set { m_InputActionsPerSecond = value; }
    }

    public bool moveOneElementPerAxisPress
    {
        get { return m_MoveOneElementPerAxisPress; }
        set { m_MoveOneElementPerAxisPress = value; }
    }

    public float repeatDelay
    {
        get { return m_RepeatDelay; }
        set { m_RepeatDelay = value; }
    }

    public float axisDeadzone
    {
        get { return m_AxisDeadzone; }
        set { m_AxisDeadzone = value; }
    }

    public bool allowMouseInput
    {
        get { return m_AllowMouseInput; }
        set { m_AllowMouseInput = value; }
    }

    public bool allowMouseInputIfTouchSupported
    {
        get { return m_AllowMouseInputIfTouchSupported; }
        set { m_AllowMouseInputIfTouchSupported = value; }
    }

    public bool allowActivationOnMobileDevice
    {
        get { return m_AllowActivationOnMobileDevice; }
        set { m_AllowActivationOnMobileDevice = value; }
    }

    public bool isTouchSupported
    {
        get { return m_IsTouchSupported; }
    }

    public bool isMouseSupported
    {
        get
        {
            if (!m_AllowMouseInput)
            {
                return false;
            }

            return (m_IsTouchSupported) ? m_AllowMouseInputIfTouchSupported : true;
        }
    }

    public int playersCount
    {
        get
        {
            return m_Players.Count;
        }
    }

    public int wifiPlayersCount
    {
        get
        {
            return m_WifiPlayers.Count;
        }
    }

    // MonoBehaviour's INTERFACE

    protected override void Awake()
    {
        base.Awake();

        m_IsTouchSupported = Input.touchSupported;

        // Refresh references.

        Initialize();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        InputSystem.onInputSystemResetEventMain += OnInputSystemResetEvent;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        InputSystem.onInputSystemResetEventMain -= OnInputSystemResetEvent;
    }

    // BUSINESS LOGIC

    public void AddPlayer(string i_PlayerName)
    {
        if (i_PlayerName == null)
        {
            return;
        }

        PlayerInput playerInput = InputSystem.GetPlayerByNameMain(i_PlayerName);
        AddPlayer(playerInput);
    }

    public void AddPlayer(PlayerInput i_PlayerInput)
    {
        if (i_PlayerInput == null)
        {
            return;
        }

        m_Players.Add(i_PlayerInput);
        RaisePlayersChangedEvent();
    }

    public void AddWifiPlayer(string i_WifiPlayerName)
    {
        if (i_WifiPlayerName == null)
        {
            return;
        }

        WiFiPlayerInput wifiPlayerInput = WiFiInputSystem.GetPlayerByNameMain(i_WifiPlayerName);
        AddWifiPlayer(wifiPlayerInput);
    }

    public void AddWifiPlayer(WiFiPlayerInput i_PlayerInput)
    {
        if (i_PlayerInput == null)
        {
            return;
        }

        m_WifiPlayers.Add(i_PlayerInput);
        RaisePlayersChangedEvent();
    }

    public void Clear()
    {
        m_Players.Clear();
        m_WifiPlayers.Clear();

        RaisePlayersChangedEvent();
    }

    public PlayerInput GetPlayerInput(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= m_Players.Count)
        {
            return null;
        }

        return m_Players[i_PlayerIndex];
    }

    public WiFiPlayerInput GetWifiPlayerInput(int i_PlayerIndex)
    {
        if (i_PlayerIndex < 0 || i_PlayerIndex >= m_WifiPlayers.Count)
        {
            return null;
        }

        return m_WifiPlayers[i_PlayerIndex];
    }

    // UTILS

    private void Initialize()
    {
        // Restore default Players

        {
            for (int playerIndex = 0; playerIndex < m_DefaultPlayers.Count; ++playerIndex)
            {
                PlayerInput playerInput = InputSystem.GetPlayerByNameMain(m_DefaultPlayers[playerIndex]);

                if (playerInput != null)
                {
                    m_Players.Add(playerInput);
                }
            }
        }

        // Restore default WiFi Players

        {
            for (int playerIndex = 0; playerIndex < m_DefaultWifiPlayers.Count; ++playerIndex)
            {
                WiFiPlayerInput playerInput = WiFiInputSystem.GetPlayerByNameMain(m_DefaultWifiPlayers[playerIndex]);

                if (playerInput != null)
                {
                    m_WifiPlayers.Add(playerInput);
                }
            }
        }
    }

    // BaseInputModule's INTERFACE

    public override void UpdateModule()
    {
        if (!InputSystem.isReadyMain)
        {
            return;
        }

        if (isMouseSupported)
        {
            m_LastMousePosition = m_MousePosition;
            m_MousePosition = InputSystem.mousePositionMain;
        }
    }

    public override bool IsModuleSupported()
    {
        if (Application.isMobilePlatform)
        {
            return m_AllowActivationOnMobileDevice || Input.mousePresent;
        }

        return true;
    }

    public override bool ShouldActivateModule()
    {
        if (!base.ShouldActivateModule())
        {
            return false;
        }

        if (!InputSystem.isReadyMain)
        {
            return false;
        }

        bool shouldActivate = false;

        for (int i = 0; i < m_Players.Count; ++i)
        {
            PlayerInput player = m_Players[i];

            if (player == null)
            {
                continue;
            }

            shouldActivate |= player.GetButtonDown(m_SubmitButton);
            shouldActivate |= player.GetButtonDown(m_CancelButton);

            if (m_MoveOneElementPerAxisPress)
            { 
                // Axis press moves only to the next UI element with each press.
                shouldActivate |= player.GetButtonDown(m_HorizontalAxis) || player.GetNegativeButtonDown(m_HorizontalAxis);
                shouldActivate |= player.GetButtonDown(m_VerticalAxis) || player.GetNegativeButtonDown(m_VerticalAxis);
            }
            else
            { 
                // Default behavior - axis press scrolls quickly through UI elements.
                shouldActivate |= !Mathf.Approximately(player.GetAxisRaw(m_HorizontalAxis), 0.0f);
                shouldActivate |= !Mathf.Approximately(player.GetAxisRaw(m_VerticalAxis), 0.0f);
            }
        }

        for (int i = 0; i < m_WifiPlayers.Count; ++i)
        {
            WiFiPlayerInput player = m_WifiPlayers[i];

            if (player == null)
            {
                continue;
            }

            shouldActivate |= player.GetButtonDown(m_WifiSubmitButton);
            shouldActivate |= player.GetButtonDown(m_WifiCancelButton);

            if (m_MoveOneElementPerAxisPress)
            {
                shouldActivate |= player.GetButtonDown(m_WifiHorizontalAxis);
                shouldActivate |= player.GetButtonDown(m_WifiVerticalAxis);
            }
            else
            {
                shouldActivate |= !Mathf.Approximately(player.GetAxis(m_WifiHorizontalAxis), 0f);
                shouldActivate |= !Mathf.Approximately(player.GetAxis(m_WifiVerticalAxis), 0f);
            }
        }

        if (isMouseSupported)
        {
            shouldActivate |= (m_MousePosition - m_LastMousePosition).sqrMagnitude > 0f;
            shouldActivate |= InputSystem.GetMouseButtonDownMain(0);
        }

        return shouldActivate;
    }

    public override void ActivateModule()
    {
        base.ActivateModule();

        if (isMouseSupported)
        {
            m_MousePosition = InputSystem.mousePositionMain;
            m_LastMousePosition = InputSystem.mousePositionMain;
        }

        GameObject toSelect = eventSystem.currentSelectedGameObject;

        if (toSelect == null)
        {
            toSelect = eventSystem.firstSelectedGameObject;
        }

        eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
    }

    public override void DeactivateModule()
    {
        base.DeactivateModule();

        ClearSelection();
    }

    public override void Process()
    {
        if (!InputSystem.isReadyMain)
        {
            return;
        }

        bool usedEvent = SendUpdateEventToSelectedObject();

        if (eventSystem.sendNavigationEvents)
        {
            if (!usedEvent)
            {
                usedEvent |= SendMoveEventToSelectedObject();
            }

            if (!usedEvent)
            {
                SendSubmitEventToSelectedObject();
            }
        }

        if (isMouseSupported)
        {
            ProcessMouseEvent();
        }
    }

    // INTERNALS

    private bool SendMoveEventToSelectedObject()
    {
        float time = Time.unscaledTime;

        // Check for zero movement and clear.

        Vector2 movement = GetRawMoveVector();
        if (Mathf.Approximately(movement.x, 0f) && Mathf.Approximately(movement.y, 0f))
        {
            m_ConsecutiveMoveCount = 0;
            return false;
        }

        // Check if movement is in the same direction as previously.

        bool similarDir = (Vector2.Dot(movement, m_LastMoveVector) > 0);

        // Check if a button/key/axis was just pressed this frame.

        bool buttonJustPressed = CheckButtonOrKeyMovement(time);

        // If user just pressed button/key/axis, always allow movement.

        bool allow = buttonJustPressed;
        if (!allow)
        {
            // Apply repeat delay and input actions per second limits.

            if (m_RepeatDelay > 0.0f)
            { 
                // Apply repeat delay. Otherwise, user held down key or axis.
                // If direction didn't change at least 90 degrees, wait for delay before allowing consequtive event.
                if (similarDir && m_ConsecutiveMoveCount == 1)
                { 
                    // This is the 2nd tick after the initial that activated the movement in this direction.
                    allow = (time > m_PrevActionTime + m_RepeatDelay);

                    // If direction changed at least 90 degree, or we already had the delay, repeat at repeat rate.
                }
                else
                {
                    allow = (time > m_PrevActionTime + 1f / m_InputActionsPerSecond); // Apply input actions per second limit.
                }
            }
            else
            { 
                // Not using a repeat delay.
                allow = (time > m_PrevActionTime + 1f / m_InputActionsPerSecond); // Apply input actions per second limit.
            }
        }

        if (!allow)
        {
            return false; // Movement not allowed, done.
        }

        // Get the axis move event.

        AxisEventData axisEventData = GetAxisEventData(movement.x, movement.y, 0.6f);
        if (axisEventData.moveDir == MoveDirection.None)
        {
            return false; // Input vector was not enough to move this cycle, done.
        }

        if (!AllowMovement(eventSystem.currentSelectedGameObject, axisEventData.moveDir))
        {
            return false;
        }

        // Execute the move.
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);

        // Update records and counters.
        if (!similarDir)
        {
            m_ConsecutiveMoveCount = 0;
        }

        m_ConsecutiveMoveCount++;

        m_PrevActionTime = time;
        m_LastMoveVector = movement;

        return axisEventData.used;
    }

    private bool CheckButtonOrKeyMovement(float time)
    {
        bool allow = false;

        for (int i = 0; i < m_Players.Count; ++i)
        {
            PlayerInput player = m_Players[i];

            if (player == null)
            {
                continue;
            }

            allow |= player.GetButtonDown(m_HorizontalAxis) || player.GetNegativeButtonDown(m_HorizontalAxis);
            allow |= player.GetButtonDown(m_VerticalAxis) || player.GetNegativeButtonDown(m_VerticalAxis);
        }

        for (int i = 0; i < m_WifiPlayers.Count; ++i)
        {
            WiFiPlayerInput player = m_WifiPlayers[i];

            if (player == null)
            {
                continue;
            }

            allow |= player.GetButtonDown(m_WifiHorizontalAxis);
            allow |= player.GetButtonDown(m_WifiVerticalAxis);
        }

        return allow;
    }

    private bool SendUpdateEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
        {
            return false;
        }

        BaseEventData data = GetBaseEventData();
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);

        return data.used;
    }

    private bool SendSubmitEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
        {
            return false;
        }

        BaseEventData data = GetBaseEventData();

        // Handle events from standard devices.

        for (int i = 0; i < m_Players.Count; ++i)
        {
            if (m_Players[i] == null)
            {
                continue;
            }

            if (m_Players[i].GetButtonDown(m_SubmitButton))
            {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);
                break;
            }

            if (m_Players[i].GetButtonDown(m_CancelButton))
            {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
                break;
            }
        }

        // Handle events from Wifi devices.

        for (int i = 0; i < m_WifiPlayers.Count; ++i)
        {
            if (m_WifiPlayers[i] == null)
            {
                continue;
            }

            if (m_WifiPlayers[i].GetButtonDown(m_WifiSubmitButton))
            {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);
                break;
            }

            if (m_WifiPlayers[i].GetButtonDown(m_WifiCancelButton))
            {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
                break;
            }
        }

        return data.used;
    }

    private Vector2 GetRawMoveVector()
    {
        Vector2 move = Vector2.zero;

        bool horizontalButton = false;
        bool verticalButton = false;

        for (int i = 0; i < m_Players.Count; i++)
        {
            PlayerInput player = m_Players[i];

            if (player == null)
            {
                continue;
            }

            if (m_MoveOneElementPerAxisPress)
            { 
                // Axis press moves only to the next UI element with each press.

                float x = 0.0f;
                if (player.GetButtonDown(m_HorizontalAxis))
                {
                    x = 1.0f;
                }
                else if (player.GetNegativeButtonDown(m_HorizontalAxis))
                {
                    x = -1.0f;
                }

                float y = 0.0f;
                if (player.GetButtonDown(m_VerticalAxis))
                {
                    y = 1.0f;
                }
                else if (player.GetNegativeButtonDown(m_VerticalAxis))
                {
                    y = -1.0f;
                }

                move.x += x;
                move.y += y;
            }
            else
            { 
                // Default behavior - axis press scrolls quickly through UI elements.

                move.x += player.GetAxisRaw(m_HorizontalAxis);
                move.y += player.GetAxisRaw(m_VerticalAxis);
            }

            horizontalButton |= player.GetButtonDown(m_HorizontalAxis) || player.GetNegativeButtonDown(m_HorizontalAxis);
            verticalButton |= player.GetButtonDown(m_VerticalAxis) || player.GetNegativeButtonDown(m_VerticalAxis);
        }

        for (int i = 0; i < m_WifiPlayers.Count; ++i)
        {
            WiFiPlayerInput player = m_WifiPlayers[i];

            if (player == null)
            {
                continue;
            }

            if (m_MoveOneElementPerAxisPress)
            {
                // Axis press moves only to the next UI element with each press.

                float x = 0.0f;
                if (player.GetPositiveButtonDown(m_WifiHorizontalAxis))
                {
                    x = 1.0f;
                }
                else if (player.GetNegativeButtonDown(m_WifiHorizontalAxis))
                {
                    x = -1.0f;
                }

                float y = 0.0f;
                if (player.GetPositiveButtonDown(m_WifiVerticalAxis))
                {
                    y = 1.0f;
                }
                else if (player.GetNegativeButtonDown(m_WifiVerticalAxis))
                {
                    y = -1.0f;
                }

                move.x += x;
                move.y += y;
            }
            else
            {
                // Default behavior - axis press scrolls quickly through UI elements.

                move.x += player.GetAxis(m_WifiHorizontalAxis);
                move.y += player.GetAxis(m_WifiVerticalAxis);
            }

            horizontalButton |= player.GetButtonDown(m_WifiHorizontalAxis);
            verticalButton |= player.GetButtonDown(m_WifiVerticalAxis);
        }

        if (horizontalButton)
        {
            if (move.x < -m_AxisDeadzone)
            {
                move.x = -1f;
            }

            if (move.x > m_AxisDeadzone)
            {
                move.x = 1f;
            }
        }

        if (verticalButton)
        {
            if (move.y < -m_AxisDeadzone)
            {
                move.y = -1f;
            }

            if (move.y > m_AxisDeadzone)
            {
                move.y = 1f;
            }
        }

        return move;
    }

    private bool AllowMovement(GameObject i_From, MoveDirection i_Dir)
    {
        if (i_From == null || i_Dir == MoveDirection.None)
        {
            return true;
        }

        Selectable selectable = i_From.GetComponent<Selectable>();

        if (selectable == null || selectable.navigation.mode != Navigation.Mode.Explicit)
        {
            return true;
        }

        Selectable to = null;

        switch (i_Dir)
        {
            case MoveDirection.Left:
                to = selectable.navigation.selectOnLeft;
                break;

            case MoveDirection.Right:
                to = selectable.navigation.selectOnRight;
                break;

            case MoveDirection.Up:
                to = selectable.navigation.selectOnUp;
                break;

            case MoveDirection.Down:
                to = selectable.navigation.selectOnDown;
                break;
        }

        return (to == null || to.IsInteractable());
    }

    private void ProcessMouseEvent()
    {
        MouseState mouseData = GetMousePointerEventData(kMouseLeftId);

        bool pressed = mouseData.AnyPressesThisFrame();
        bool released = mouseData.AnyReleasesThisFrame();

        MouseButtonEventData leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

        if (!UseMouse(pressed, released, leftButtonData.buttonData))
            return;

        // Process the first mouse button fully
        ProcessMousePress(leftButtonData);
        ProcessMove(leftButtonData.buttonData);
        ProcessDrag(leftButtonData.buttonData);

        // Now process right / middle clicks
        ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData);
        ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
        ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
        ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);

        if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
        {
            var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
            ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
        }
    }

    private void ProcessMousePress(MouseButtonEventData data)
    {
        PointerEventData pointerEvent = data.buttonData;
        GameObject currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

        if (data.PressedThisFrame())
        {
            pointerEvent.eligibleForClick = true;
            pointerEvent.delta = Vector2.zero;
            pointerEvent.dragging = false;
            pointerEvent.useDragThreshold = true;
            pointerEvent.pressPosition = pointerEvent.position;
            pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

            DeselectIfSelectionChanged(currentOverGo, pointerEvent);

            GameObject newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

            if (newPressed == null)
                newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            float time = Time.unscaledTime;

            if (newPressed == pointerEvent.lastPress)
            {
                float diffTime = time - pointerEvent.clickTime;
                if (diffTime < 0.3f)
                {
                    ++pointerEvent.clickCount;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.clickTime = time;
            }
            else
            {
                pointerEvent.clickCount = 1;
            }

            pointerEvent.pointerPress = newPressed;
            pointerEvent.rawPointerPress = currentOverGo;

            pointerEvent.clickTime = time;

            pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

            if (pointerEvent.pointerDrag != null)
            {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }
        }

        if (data.ReleasedThisFrame())
        {
            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

            GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
            {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
            }
            else if (pointerEvent.pointerDrag != null)
            {
                ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
            }

            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = null;
            pointerEvent.rawPointerPress = null;

            if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
            }

            pointerEvent.dragging = false;
            pointerEvent.pointerDrag = null;

            if (currentOverGo != pointerEvent.pointerEnter)
            {
                HandlePointerExitAndEnter(pointerEvent, null);
                HandlePointerExitAndEnter(pointerEvent, currentOverGo);
            }
        }
    }

    private void OnInputSystemResetEvent()
    {
        // Backup players list.

        List<string> playerNames = new List<string>();

        for (int index = 0; index < m_Players.Count; ++index)
        {
            PlayerInput playerInput = m_Players[index];
            if (playerInput != null)
            {
                string playerName = playerInput.Name;
                playerNames.Add(playerName);
            }
        }

        // Clear players list.

        m_Players.Clear();

        // Re-Init players and assignments.

        for (int index = 0; index < playerNames.Count; ++index)
        {
            string playerName = playerNames[index];
            AddPlayer(playerName);
        }
    }

    // UTILS

    private void RaisePlayersChangedEvent()
    {
        if (playersChangedEvent != null)
        {
            playersChangedEvent();
        }
    }

    // STATIC

    private static bool UseMouse(bool pressed, bool released, PointerEventData pointerData)
    {
        return (pressed || released || pointerData.IsPointerMoving() || pointerData.IsScrolling());
    }
}