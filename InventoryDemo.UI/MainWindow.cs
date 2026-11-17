using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using CharacterMainLib;
using DemoGameLib;
using InventoryCoreLib;

using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.UI;

public sealed partial class MainWindow : Window
{
    // Model update runs every second. UI refresh never performs game actions.
    private CancellationTokenSource? _gameLoopCts;
    private Task? _gameLoopTask;

    private async Task GameLoop(CancellationToken token)
    {
        const float deltaTime = 1.0f;

        try
        {
            while (true)
            {
                // Wait first: startup refresh has already been done in constructor.
                await Task.Delay(TimeSpan.FromSeconds(1), token);

                // The optional scenario has its own simulation clock.
                // Avoid updating effects twice while it is running.
                if (_scenarioCancellation is null)
                {
                    _character.Update(deltaTime);
                    _currentFoe.Actor.Update(deltaTime);
                }

                // Reuse UI controls; update text and changed items only.
                RefreshCurrentState();
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Expected when the window closes.
        }
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (_gameLoopCts is not null)
            return;

        _gameLoopCts = new CancellationTokenSource();
        _gameLoopTask = GameLoop(_gameLoopCts.Token);
    }

    protected override void OnClosed(EventArgs e)
    {
        _gameLoopCts?.Cancel();

        base.OnClosed(e);
    }

    //
    private const double DesignWidth = 1920;
    private const double DesignHeight = 1080;

    private static readonly IBrush Gold = new SolidColorBrush(Color.Parse("#D89B32"));
    private static readonly IBrush WarmGold = new SolidColorBrush(Color.Parse("#F2B84A"));
    private static readonly IBrush DarkBrown = new SolidColorBrush(Color.Parse("#1B110A"));
    private static readonly IBrush PanelBlack = new SolidColorBrush(Color.Parse("#050505"));
    private static readonly IBrush ParchmentText = new SolidColorBrush(Color.Parse("#4A321D"));

    private readonly DemoCharacter _character;
    private FoePresentation _currentFoe;

    private readonly TextBox _consoleBox = new();
    private readonly Canvas _inventoryCanvas = new() { Width = 705, Height = 415 };
    private readonly Canvas _equipmentCanvas = new() { Width = 430, Height = 365 };
    private readonly ComboBox _pickupCombo = new();

    // Persistent inventory/equipment controls. They are not replaced every tick.
    private bool _inventoryLayoutCreated;
    private bool _equipmentLayoutCreated;

    private readonly Dictionary<(ushort Row, ushort Col), Border> _inventoryCells = new();
    private readonly Dictionary<(ushort Row, ushort Col), IOwnedUsable?> _visibleInventoryItems = new();
    private readonly Dictionary<(ushort Row, ushort Col), (Border Host, TextBlock Label)> _inventoryBadges = new();
    private readonly Dictionary<(ushort Row, ushort Col), TextBlock> _inventoryTooltipInfos = new();

    private readonly Dictionary<string, Border> _equipmentSlots = new(StringComparer.Ordinal);
    private readonly Dictionary<string, IOwnedEquipable?> _visibleEquipmentItems = new(StringComparer.Ordinal);
    private (Border Host, TextBlock Label)? _ammoBadge;
    private TextBlock? _ammoTooltipInfo;

    private readonly TextBlock _foeName = new();
    private readonly TextBlock _foeStatus = new();
    private readonly Image _foeImage = new();
    private readonly Image _playerAvatar = new();

    private readonly TextBlock _healthText = new();
    private readonly TextBlock _manaText = new();
    private readonly TextBlock _staminaText = new();
    private readonly TextBlock _attackText = new();
    private readonly TextBlock _defenceText = new();
    private readonly TextBlock _statsText = new();
    private readonly TextBlock _playerNameText = new();

    private Position? _selectedPosition;
    private Border? _selectedCell;

    private CancellationTokenSource? _scenarioCancellation;
    private Task? _scenarioTask;

    private string _playerAvatarPath = "PlayerCharacterAvatar/DefaultPlayer.png";

    public MainWindow()
    {
        Title = "RPG Inventory Demo";
        Width = 1280;
        Height = 720;

        MinWidth = 1000;
        MinHeight = 650;

        WindowState = WindowState.Normal;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        CanResize = false;
        Background = DarkBrown;

        _character = CreateCharacter();

        var dummy = new TrainingDummy("Goblin Assasin", 250);
        _currentFoe = new FoePresentation(
            dummy.Name,
            dummy,
            "Foes Miniatures/GoblinAssasin.png",
            dummy.Update,
            () => $"HP: {dummy.Health}/{dummy.MaxHealth}");

        Content = new Viewbox
        {
            Stretch = Stretch.Uniform,
            StretchDirection = StretchDirection.Both,
            Child = BuildDesignCanvas()
        };

        foreach (string line in LogHub.History)
            AppendLog(line);

        LogHub.LineAdded += OnLogLineAdded;
        Closed += OnWindowClosed;

        _pickupCombo.ItemsSource = GameCore.Entries;
        _pickupCombo.SelectedIndex = 0;

        ConfigureDemo();

        _character.DamageVital("Health", 80);
        _character.DamageVital("Mana", 80);
        RefreshAll(0.0f);
    }

    #region Public customization entry points

    public DemoCharacter Character => _character;

    public FoePresentation CurrentFoe => _currentFoe;

    public void SetCurrentFoe(FoePresentation foe)
    {
        _currentFoe = foe ?? throw new ArgumentNullException(nameof(foe));
        Console.WriteLine($"Current foe: {_currentFoe.Name}.");
        RefreshFoe();
    }

    public void SetPlayerAvatar(string resourcePath)
    {
        _playerAvatarPath = resourcePath;
        RefreshPlayerAvatar();
    }

    #endregion

    #region [ CHARACTER CREATION FUNC ]

    private static DemoCharacter CreateCharacter()
    {
        //return DemoGameLib.CharacterCreator.CreateDefault();

        return DemoGameLib.CharacterCreator.CreateFromJson(
            "Rouge", CharacterConstants.INVENTORY_SIZE);
    }

    #endregion

    #region Layout

    private Canvas BuildDesignCanvas()
    {
        var root = new Canvas
        {
            Width = DesignWidth,
            Height = DesignHeight,
            Background = DarkBrown
        };

        var background = CreateImage("UserInterfaceElements/BackgroundTristram.png", Stretch.Fill);
        Place(root, background, 0, 0, DesignWidth, DesignHeight);

        Place(root, BuildEnemyPanel(), 25, 20, 560, 610);
        Place(root, BuildConsolePanel(), 605, 20, 560, 610);
        Place(root, BuildCharacterPanel(), 1190, 20, 705, 610);
        Place(root, BuildActionPanel(), 25, 650, 560, 405);
        Place(root, BuildPlayerAndPickupPanel(), 605, 650, 560, 405);
        Place(root, BuildInventoryPanel(), 1190, 650, 705, 405);

        return root;
    }

    private Control BuildEnemyPanel()
    {
        var canvas = new Canvas { Width = 560, Height = 610 };

        var frame = CreateImage("UserInterfaceElements/EnemyFrame.png", Stretch.Fill);
        Place(canvas, frame, 0, 0, 560, 610);

        var imageHost = new Border
        {
            Background = Brushes.Transparent,
            ClipToBounds = true,
            CornerRadius = new CornerRadius(8),
            Child = _foeImage
        };
        _foeImage.Stretch = Stretch.UniformToFill;
        Place(canvas, imageHost, 88, 112, 385, 445);

        _foeName.Foreground = WarmGold;
        _foeName.FontSize = 29;
        _foeName.TextAlignment = TextAlignment.Center;
        _foeName.HorizontalAlignment = HorizontalAlignment.Stretch;
        Place(canvas, _foeName, 92, 42, 375, 42);

        _foeStatus.Foreground = Brushes.White;
        _foeStatus.FontSize = 17;
        _foeStatus.TextAlignment = TextAlignment.Center;
        Place(canvas, _foeStatus, 115, 525, 330, 28);

        return canvas;
    }

    private Control BuildConsolePanel()
    {
        var outer = new Border
        {
            Background = DarkBrown,
            BorderBrush = Gold,
            BorderThickness = new Thickness(2),
            Padding = new Thickness(14),
            CornerRadius = new CornerRadius(4)
        };

        var grid = new Grid
        {
            RowDefinitions = new RowDefinitions
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(new GridLength(1, GridUnitType.Star))
            }
        };

        var title = new TextBlock
        {
            Text = "Console",
            FontSize = 46,
            Foreground = Brushes.White,
            Margin = new Thickness(20, 5, 0, 8)
        };

        _consoleBox.IsReadOnly = true;
        _consoleBox.AcceptsReturn = true;
        _consoleBox.TextWrapping = TextWrapping.Wrap;
        _consoleBox.Background = PanelBlack;
        _consoleBox.Foreground = new SolidColorBrush(Color.Parse("#ECE6DA"));
        _consoleBox.BorderThickness = new Thickness(0);
        _consoleBox.FontSize = 16;
        _consoleBox.Padding = new Thickness(16);
        ScrollViewer.SetVerticalScrollBarVisibility(_consoleBox, ScrollBarVisibility.Auto);
        ScrollViewer.SetHorizontalScrollBarVisibility(_consoleBox, ScrollBarVisibility.Disabled);

        Grid.SetRow(title, 0);
        Grid.SetRow(_consoleBox, 1);
        grid.Children.Add(title);
        grid.Children.Add(_consoleBox);
        outer.Child = grid;
        return outer;
    }

    private Control BuildCharacterPanel()
    {
        var canvas = new Canvas { Width = 705, Height = 610 };

        var equipmentFrame = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(150, 15, 13, 12)),
            BorderBrush = Gold,
            BorderThickness = new Thickness(1)
        };
        equipmentFrame.Child = _equipmentCanvas;
        Place(canvas, equipmentFrame, 0, 0, 430, 365);

        var statsPanel = BuildStatsPanel();
        Place(canvas, statsPanel, 445, 0, 260, 500);

        Place(canvas, BuildCombatValueBar("UserInterfaceElements/AttackIcon.png", _attackText), 18, 390, 390, 72);
        Place(canvas, BuildCombatValueBar("UserInterfaceElements/DefenceIcon.png", _defenceText), 18, 475, 390, 72);

        return canvas;
    }

    private Control BuildStatsPanel()
    {
        var canvas = new Canvas { Width = 260, Height = 500 };
        var parchment = CreateImage("UserInterfaceElements/StatsParchment.png", Stretch.Fill);
        Place(canvas, parchment, 0, 0, 260, 500);

        var heading = new TextBlock
        {
            Text = "Stats",
            Foreground = ParchmentText,
            FontSize = 26,
            FontWeight = FontWeight.SemiBold,
            TextAlignment = TextAlignment.Center
        };
        Place(canvas, heading, 45, 55, 170, 36);

        _statsText.Foreground = ParchmentText;
        _statsText.FontSize = 20;
        _statsText.LineHeight = 35;
        Place(canvas, _statsText, 55, 112, 155, 270);

        _playerNameText.Foreground = new SolidColorBrush(Color.Parse("#21160E"));
        _playerNameText.FontSize = 23;
        _playerNameText.FontWeight = FontWeight.SemiBold;
        _playerNameText.TextAlignment = TextAlignment.Center;
        Place(canvas, _playerNameText, 28, 410, 205, 42);

        return canvas;
    }

    private Control BuildCombatValueBar(string iconPath, TextBlock valueText)
    {
        var canvas = new Canvas { Width = 390, Height = 72 };
        var icon = CreateImage(iconPath, Stretch.Uniform);
        Place(canvas, icon, 0, 0, 72, 72);

        var bar = CreateImage("UserInterfaceElements/TitleBanner.png", Stretch.Fill);
        Place(canvas, bar, 76, 4, 305, 64);

        valueText.Foreground = WarmGold;
        valueText.FontSize = 31;
        valueText.TextAlignment = TextAlignment.Center;
        valueText.VerticalAlignment = VerticalAlignment.Center;
        Place(canvas, valueText, 118, 15, 215, 42);
        return canvas;
    }

    private Control BuildActionPanel()
    {
        var canvas = new Canvas { Width = 560, Height = 405 };
        var wood = CreateImage("UserInterfaceElements/WoodBackground.jpg", Stretch.Fill);
        wood.Opacity = 0.92;
        Place(canvas, wood, 0, 0, 560, 405);

        var frame = new Border
        {
            BorderBrush = Gold,
            BorderThickness = new Thickness(2),
            Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0))
        };
        Place(canvas, frame, 0, 0, 560, 405);

        var header = CreateHeader("Actions");
        Place(canvas, header, 145, 0, 270, 66);

        string[] captions =
        {
            "Attack Prim", "Att Second", "Restore foe",
            "4", "5", "6",
            "7", "8", "9",
            "10", "11", "12"
        };

        Action[] actions =
        {
            Action01PrimaryAttack, Action02SecondaryAttack, Action03RestoreFoe,
            Action04Scenario, Action05, Action06,
            Action07, Action08, Action09,
            Action10, Action11, Action12
        };

        const double startX = 20;
        const double startY = 80;
        const double buttonW = 165;
        const double buttonH = 68;
        const double gapX = 13;
        const double gapY = 17;

        for (int i = 0; i < 12; i++)
        {
            int row = i / 3;
            int col = i % 3;
            var button = CreateFantasyButton(captions[i], actions[i]);
            Place(canvas, button,
                startX + col * (buttonW + gapX),
                startY + row * (buttonH + gapY),
                buttonW,
                buttonH);
        }

        return canvas;
    }

    private Control BuildPlayerAndPickupPanel()
    {
        var canvas = new Canvas { Width = 560, Height = 405 };
        var wood = CreateImage("UserInterfaceElements/WoodBackground.jpg", Stretch.Fill);
        wood.Opacity = 0.90;
        Place(canvas, wood, 0, 0, 560, 405);

        var frame = new Border
        {
            BorderBrush = Gold,
            BorderThickness = new Thickness(2),
            Background = new SolidColorBrush(Color.FromArgb(95, 0, 0, 0))
        };
        Place(canvas, frame, 0, 0, 560, 405);

        var avatarHost = new Border
        {
            Background = Brushes.Black,
            BorderBrush = Gold,
            BorderThickness = new Thickness(2),
            ClipToBounds = true,
            Child = _playerAvatar
        };
        _playerAvatar.Stretch = Stretch.UniformToFill;
        Place(canvas, avatarHost, 25, 35, 185, 245);

        Place(canvas, BuildVitalBar("UserInterfaceElements/HealthIcon.png", _healthText), 225, 35, 315, 72);
        Place(canvas, BuildVitalBar("UserInterfaceElements/ManaIcon.png", _manaText), 225, 125, 315, 72);
        Place(canvas, BuildVitalBar("UserInterfaceElements/StaminaIcon.png", _staminaText), 225, 215, 315, 72);

        _pickupCombo.FontSize = 19;
        _pickupCombo.Background = new SolidColorBrush(Color.Parse("#D5B37A"));
        _pickupCombo.Foreground = new SolidColorBrush(Color.Parse("#2B1A0C"));
        _pickupCombo.HorizontalContentAlignment = HorizontalAlignment.Center;
        _pickupCombo.PlaceholderText = "Drop-list items";
        Place(canvas, _pickupCombo, 25, 315, 300, 58);

        var pickButton = CreateFantasyButton("Pick Item", PickUpSelected);
        Place(canvas, pickButton, 340, 308, 195, 70);

        return canvas;
    }

    private Control BuildVitalBar(string iconPath, TextBlock text)
    {
        var canvas = new Canvas { Width = 315, Height = 72 };
        var icon = CreateImage(iconPath, Stretch.Uniform);
        Place(canvas, icon, 0, 0, 72, 72);

        var bar = CreateImage("UserInterfaceElements/TitleBanner.png", Stretch.Fill);
        Place(canvas, bar, 70, 4, 238, 64);

        text.Foreground = WarmGold;
        text.FontSize = 28;
        text.TextAlignment = TextAlignment.Center;
        Place(canvas, text, 105, 17, 170, 40);
        return canvas;
    }

    private Control BuildInventoryPanel()
    {
        var border = new Border
        {
            Background = Brushes.Transparent,
            Child = _inventoryCanvas
        };
        return border;
    }

    private Control CreateHeader(string text)
    {
        var canvas = new Canvas();
        var bg = CreateImage("UserInterfaceElements/TitleBanner.png", Stretch.Fill);
        Place(canvas, bg, 0, 0, 270, 66);
        var title = new TextBlock
        {
            Text = text,
            Foreground = WarmGold,
            FontSize = 28,
            FontWeight = FontWeight.SemiBold,
            TextAlignment = TextAlignment.Center
        };
        Place(canvas, title, 40, 16, 190, 40);
        return canvas;
    }

    private Button CreateFantasyButton(string text, Action action)
    {
        var content = new Grid();
        content.Children.Add(CreateImage("UserInterfaceElements/CtaButton.png", Stretch.Fill));
        content.Children.Add(new TextBlock
        {
            Text = text,
            Foreground = new SolidColorBrush(Color.Parse("#21150B")),
            FontSize = 20,
            FontWeight = FontWeight.SemiBold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center
        });

        var button = new Button
        {
            Content = content,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(0),
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Stretch
        };
        button.Click += (_, _) => action();
        return button;
    }

    #endregion

    #region Refresh

    // A presentation-only refresh. In particular it MUST NOT call
    // character.Update() or any weapon attack method.
    // The parameter is kept so existing partial-class callers still compile.
    private void RefreshAll(float dTime)
    {
        RefreshFoe();
        RefreshPlayerAvatar();
        RefreshCurrentState();
    }

    // Safe to run every second while a tooltip is open.
    private void RefreshCurrentState()
    {
        _foeStatus.Text = _currentFoe.StatusText?.Invoke() ?? string.Empty;
        RefreshVitals();
        RefreshStats();
        RefreshCombatValues();
        RefreshEquipment();
        RefreshInventory();
    }

    private void RefreshFoe()
    {
        _foeName.Text = _currentFoe.Name;
        _foeStatus.Text = _currentFoe.StatusText?.Invoke() ?? string.Empty;
        _foeImage.Source = AssetLocator.LoadBitmap(_currentFoe.MiniaturePath);
    }

    private void RefreshPlayerAvatar()
        => _playerAvatar.Source = AssetLocator.LoadBitmap(_playerAvatarPath);

    private void RefreshVitals()
    {
        _healthText.Text = _character.VitalStats.TryGetValue("Health", out var hp) ? hp.ToString() : "--/--";
        _manaText.Text = _character.VitalStats.TryGetValue("Mana", out var mana) ? mana.ToString() : "--/--";
        _staminaText.Text = _character.VitalStats.TryGetValue("Stamina", out var stamina) ? stamina.ToString() : "--/--";
    }

    private void RefreshStats()
    {
        uint Stat(string name) => _character.AttributeStats.TryGetValue(name, out var value) ? value.CurrentValue : 0;

        _statsText.Text =
            $"Str:     {Stat("Strength")}\n" +
            $"Dex:     {Stat("Dexterity")}\n" +
            $"Int:     {Stat("Intelligence")}\n" +
            $"Will:    {Stat("Willpower")}\n" +
            $"Endr:    {Stat("Endurance")}\n" +
            $"Luck:    {Stat("Luck")}\n" +
            $"Speed:   {_character.CurrMoveSpeed:0.0}";

        _playerNameText.Text = _character.Name;
    }

    private void RefreshCombatValues()
    {
        _defenceText.Text = _character.Equipment.Defence.ToString();

        // Read the weapon DEFINITION, never invoke PrimaryAttack here!
        // PrimaryAttack may spend ammo / durability even when no button was pressed.
        uint maxDamage = 0;
        if (_character.Equipment.ItemAtSlot("Weapon") is IOwnedEquipable equippedWeapon &&
            GetDefinition(equippedWeapon) is BaseWeapon definition)
        {
            maxDamage = definition.PrimaryAttack.MaxDamage;
        }

        _attackText.Text = maxDamage.ToString();
    }

    private void RefreshInventory()
    {
        const double startX = 77;
        const double startY = 83;
        const double cellW = 91;
        const double cellH = 73;
        const double stepX = 93;
        const double stepY = 75;

        if (!_inventoryLayoutCreated)
        {
            _inventoryCanvas.Children.Clear();
            var frame = CreateImage("UserInterfaceElements/InventoryFrame.png", Stretch.Fill);
            Place(_inventoryCanvas, frame, 0, 0, 705, 415);

            for (ushort row = 0; row < 4; row++)
            {
                for (ushort col = 0; col < 6; col++)
                {
                    var pos = new Position(row, col);
                    Border cell = CreateInventoryCell(pos, null);
                    var key = (row, col);
                    _inventoryCells.Add(key, cell);

                    Place(_inventoryCanvas, cell,
                        startX + col * stepX,
                        startY + row * stepY,
                        cellW,
                        cellH);
                }
            }

            _inventoryLayoutCreated = true;
        }

        // Same item reference -> same Border, icon, context menu and tooltip.
        // Only the count badge changes as the stack is consumed.
        for (ushort row = 0; row < 4; row++)
        {
            for (ushort col = 0; col < 6; col++)
            {
                var key = (row, col);
                var pos = new Position(row, col);
                IOwnedUsable? item = _character.Inventory.ItemAt(pos);
                Border cell = _inventoryCells[key];

                if (!_visibleInventoryItems.TryGetValue(key, out var previous) ||
                    !ReferenceEquals(previous, item))
                {
                    _visibleInventoryItems[key] = item;
                    _inventoryBadges.Remove(key);
                    cell.Child = CreateInventoryItemContent(item, out var badgeHost, out var badgeLabel);

                    if (badgeHost is not null && badgeLabel is not null)
                        _inventoryBadges[key] = (badgeHost, badgeLabel);

                    if (item is null)
                    {
                        ToolTip.SetTip(cell, null);
                        _inventoryTooltipInfos.Remove(key);
                        cell.ContextMenu = null;
                    }
                    else
                    {
                        ToolTip.SetTip(cell, CreateItemToolTip(item, out var infoLabel));
                        _inventoryTooltipInfos[key] = infoLabel;
                        cell.ContextMenu = BuildInventoryContextMenu(pos, item);
                    }
                }

                if (item is IStackable stackable &&
                    _inventoryBadges.TryGetValue(key, out var badge))
                {
                    badge.Label.Text = stackable.CurrentStackCount.ToString();
                    badge.Host.IsVisible = stackable.CurrentStackCount > 1;
                }

                if (item is not null && _inventoryTooltipInfos.TryGetValue(key, out var info))
                    UpdateItemTooltipInfo(info, item);
            }
        }
    }

    private Border CreateInventoryCell(Position pos, IOwnedUsable? item)
    {
        bool selected = _selectedPosition is Position current &&
                        current.Row == pos.Row && current.Column == pos.Column;

        // The cell itself is created once; its content is replaced only if
        // the item instance occupying this position actually changes.
        var cell = new Border
        {
            Background = Brushes.Transparent,
            BorderBrush = selected ? Brushes.White : Brushes.Transparent,
            BorderThickness = selected ? new Thickness(2) : new Thickness(0),
            Padding = new Thickness(4),
            Child = new Border { Background = Brushes.Transparent }
        };

        cell.PointerPressed += (_, _) => SelectInventoryCell(pos, cell);
        cell.DoubleTapped += (_, _) => UseInventoryItem(pos, "Double-click");
        return cell;
    }

    // Keep the original one-argument signature for partial-class compatibility.
    private Control CreateInventoryItemContent(IOwnedUsable? item)
        => CreateInventoryItemContent(item, out _, out _);

    private Control CreateInventoryItemContent(
        IOwnedUsable? item, out Border? badgeHost, out TextBlock? badgeLabel)
    {
        badgeHost = null;
        badgeLabel = null;
        if (item is null)
            return new Border { Background = Brushes.Transparent };

        var grid = new Grid();
        BaseItem? definition = GetDefinition(item);
        grid.Children.Add(CreateImage(definition?.Icon, Stretch.Uniform));

        if (item is IStackable stackable)
        {
            (badgeHost, badgeLabel) = CreateStackBadge();
            badgeLabel.Text = stackable.CurrentStackCount.ToString();
            badgeHost.IsVisible = stackable.CurrentStackCount > 1;
            grid.Children.Add(badgeHost);
        }

        return grid;
    }

    private static (Border Host, TextBlock Label) CreateStackBadge()
    {
        var label = new TextBlock
        {
            Foreground = Brushes.White,
            FontSize = 14,
            FontWeight = FontWeight.Bold
        };

        var host = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(185, 0, 0, 0)),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
            Padding = new Thickness(5, 1),
            Child = label
        };

        return (host, label);
    }

    private ToolTip CreateItemToolTip(IOwnedUsable item)
        => CreateItemToolTip(item, out _);

    private ToolTip CreateItemToolTip(IOwnedUsable item, out TextBlock infoLabel)
    {
        Control content = CreateItemInfoToolTip(item, out infoLabel);
        return new ToolTip
        {
            Width = 430,
            Height = 430,
            MinWidth = 430,
            MinHeight = 430,
            MaxWidth = 430,
            MaxHeight = 430,
            Padding = new Thickness(0),
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Content = content
        };
    }

    #region [ CANVAS ]

    private void RefreshEquipment()
    {
        if (!_equipmentLayoutCreated)
        {
            _equipmentCanvas.Children.Clear();
            var panel = CreateImage("UserInterfaceElements/EquipmentPanel.png", Stretch.Fill);
            Place(_equipmentCanvas, panel, 0, 0, 430, 336);

            // Original positioning, including Ammo beneath the weapon.
            AddEquipmentSlot("Head", 9, 26, 79, 92);
            AddEquipmentSlot("Gloves", 9, 120, 79, 90);
            AddEquipmentSlot("Boots", 9, 210, 79, 98);
            AddEquipmentSlot("Weapon", 96, 26, 77, 215);
            AddEquipmentSlot("Ammo", 96, 240, 76, 88);
            AddEquipmentSlot("Body", 176, 26, 106, 223);
            AddEquipmentSlot("Shield", 284, 26, 78, 214);
            AddEquipmentSlot("Amulet", 367, 72, 54, 74);
            AddEquipmentSlot("Ring", 367, 145, 54, 73);

            _equipmentLayoutCreated = true;
        }

        foreach (var slot in _equipmentSlots)
        {
            string slotName = slot.Key;
            Border border = slot.Value;
            IOwnedEquipable? item = _character.Equipment.ItemAtSlot(slotName);

            if (!_visibleEquipmentItems.TryGetValue(slotName, out var previous) ||
                !ReferenceEquals(previous, item))
            {
                _visibleEquipmentItems[slotName] = item;
                if (slotName == "Ammo")
                {
                    _ammoBadge = null;
                    _ammoTooltipInfo = null;
                }

                border.Child = CreateEquipmentItemContent(
                    item, slotName == "Ammo", out var badgeHost, out var badgeLabel);

                if (slotName == "Ammo" && badgeHost is not null && badgeLabel is not null)
                    _ammoBadge = (badgeHost, badgeLabel);

                if (item is null)
                {
                    ToolTip.SetTip(border, null);
                }
                else
                {
                    ToolTip.SetTip(border, CreateItemToolTip(item, out var infoLabel));
                    if (slotName == "Ammo")
                        _ammoTooltipInfo = infoLabel;
                }
            }

            if (slotName == "Ammo" && item is not null && _ammoTooltipInfo is not null)
                UpdateItemTooltipInfo(_ammoTooltipInfo, item);

            // Update the ammo counter in place; do not destroy an open tooltip.
            if (slotName == "Ammo" && _ammoBadge is { } badge && item is IStackable stackable)
            {
                badge.Label.Text = stackable.CurrentStackCount.ToString();
                badge.Host.IsVisible = true; // Also show 1 remaining arrow/bolt.
            }
        }
    }

    private void AddEquipmentSlot(string slotName, double x, double y, double width, double height)
    {
        var border = new Border
        {
            Background = Brushes.Transparent,
            Padding = new Thickness(2),
            Child = new Border { Background = Brushes.Transparent }
        };

        // Read the currently equipped item when clicked; never capture an old one.
        border.DoubleTapped += (_, _) =>
        {
            if (_character.Equipment.ItemAtSlot(slotName) is IOwnedEquipable equipped)
                UseEquippedItem(slotName, equipped);
        };

        _equipmentSlots.Add(slotName, border);
        Place(_equipmentCanvas, border, x, y, width, height);
    }

    private Control CreateEquipmentItemContent(IOwnedEquipable? item)
        => CreateEquipmentItemContent(item, false, out _, out _);

    private Control CreateEquipmentItemContent(
        IOwnedEquipable? item,
        bool isAmmoSlot,
        out Border? badgeHost,
        out TextBlock? badgeLabel)
    {
        badgeHost = null;
        badgeLabel = null;
        if (item is null)
            return new Border { Background = Brushes.Transparent };

        var grid = new Grid();
        BaseItem? definition = GetDefinition(item);
        grid.Children.Add(CreateImage(definition?.Miniature2D, Stretch.Uniform));

        if (isAmmoSlot && item is IStackable stackable)
        {
            (badgeHost, badgeLabel) = CreateStackBadge();
            badgeLabel.Text = stackable.CurrentStackCount.ToString();
            badgeHost.IsVisible = true;
            grid.Children.Add(badgeHost);
        }

        return grid;
    }

    #endregion

    #endregion

    #region Item interaction

    private void SelectInventoryCell(Position pos, Border cell)
    {
        if (_selectedCell is not null)
        {
            _selectedCell.BorderBrush = Brushes.Transparent;
            _selectedCell.BorderThickness = new Thickness(0);
        }

        _selectedPosition = pos;
        _selectedCell = cell;
        cell.BorderBrush = Brushes.White;
        cell.BorderThickness = new Thickness(2);
    }

    private void UseInventoryItem(Position pos, string source)
    {
        IOwnedUsable? item = _character.Inventory.ItemAt(pos);
        if (item is null)
            return;

        bool equipable = item is IOwnedEquipable;
        Console.WriteLine($"{_character.Name} uses {item.GetDefinitionName()} ({source}).");

        _character.UseItem(pos, _character);

        if (equipable && item is IOwnedEquipable equipment && !IsItemEquipped(equipment))
        {
            IOwnedUsable? rollback = _character.Inventory.AddItem(equipment, pos);
            if (rollback is not null)
                _character.Inventory.AddItem(rollback);

            Console.WriteLine($"Equip failed for {item.GetDefinitionName()}; item returned to inventory.");
        }

        RefreshAll(0.0f);
    }

    private void UseEquippedItem(string slotName, IOwnedEquipable item)
    {
        if (!HasFreeInventoryCell())
        {
            Console.WriteLine($"Cannot unequip {item.GetDefinitionName()}: inventory is full.");
            return;
        }

        Console.WriteLine($"{_character.Name} uses equipped item {item.GetDefinitionName()} ({slotName}).");
        ((ICanUseItems<IOwnedUsable>)_character).UseItem(item, _character);
        RefreshAll(0.0f);
    }

    private ContextMenu BuildInventoryContextMenu(Position pos, IOwnedUsable item)
    {
        var menuItems = new List<MenuItem>();

        var use = new MenuItem { Header = "Use" };
        use.Click += (_, _) => UseInventoryItem(pos, "Context menu: Use");
        menuItems.Add(use);

        var drop = new MenuItem { Header = "Drop" };
        drop.Click += (_, _) => DropInventoryItem(pos);
        menuItems.Add(drop);

        if (item is IOwnedEquipable)
        {
            var equip = new MenuItem { Header = "Equip" };
            equip.Click += (_, _) => UseInventoryItem(pos, "Context menu: Equip");
            menuItems.Add(equip);
        }

        ExtendInventoryContextMenu(menuItems, pos, item);

        return new ContextMenu { ItemsSource = menuItems };
    }

    private void DropInventoryItem(Position pos)
    {
        IOwnedUsable? item = _character.Inventory.RemoveItem(pos);
        if (item is null)
            return;

        _character.Inventory.RemoveItemOwnership(ref item);
        item?.SetMyTarget(null);
        Console.WriteLine($"{_character.Name} dropped {item?.GetDefinitionName() ?? "item"}.");
        RefreshAll(0.0f);
    }

    private bool IsItemEquipped(IOwnedEquipable item)
        => _character.Equipment.SlotNames.Any(slot =>
            ReferenceEquals(_character.Equipment.ItemAtSlot(slot), item));

    private bool HasFreeInventoryCell()
    {
        foreach (IOwnedUsable? item in _character.Inventory.Container)
        {
            if (item is null)
                return true;
        }
        return false;
    }

    private Control CreateItemInfoToolTip(IOwnedUsable item)
        => CreateItemInfoToolTip(item, out _);

    private Control CreateItemInfoToolTip(IOwnedUsable item, out TextBlock infoLabel)
    {
        const double width = 430;
        const double height = 430;

        var canvas = new Canvas
        {
            Width = width,
            Height = height
        };

        // Frame — оригінальний розмір 430x430
        var frame = CreateImage(
            "UserInterfaceElements/ItemInfoFrame.png",
            Stretch.None);

        Place(canvas, frame, 0, 0, width, height);


        BaseItem? definition = GetDefinition(item);


        // ============================================================
        // MINIATURE
        // ============================================================

        var miniature = CreateImage(
            definition?.Miniature2D,
            Stretch.Uniform);

        Place(
            canvas,
            miniature,
            48,     // X
            92,     // Y
            115,    // Width
            115);   // Height


        // ============================================================
        // ITEM NAME
        // ============================================================

        var name = new TextBlock
        {
            Text = definition?.Name
                   ?? item.GetDefinitionName(),

            Foreground = ParchmentText,

            FontSize = 21,
            FontWeight = FontWeight.Bold,

            TextWrapping = TextWrapping.Wrap,

            TextAlignment = TextAlignment.Left
        };

        Place(
            canvas,
            name,
            180,
            90,
            195,
            50);


        // ============================================================
        // BASIC INFO
        // ============================================================

        var info = new TextBlock
        {
            Text = BuildItemTooltipInfo(item),

            Foreground = ParchmentText,

            FontSize = 16,
            LineHeight = 26,

            TextWrapping = TextWrapping.NoWrap
        };

        Place(
            canvas, info,
            180,
            145,
            190,
            90);

        infoLabel = info;


        // ============================================================
        // DESCRIPTION
        // ============================================================

        var description = new TextBlock
        {
            Text = definition?.Descriprion
                   ?? string.Empty,

            Foreground = ParchmentText,

            FontSize = 15,

            LineHeight = 21,

            TextWrapping = TextWrapping.Wrap,

            TextAlignment = TextAlignment.Left
        };

        Place(canvas, description, 52, 275, 325, 90);

        return canvas;
    }

    private static string BuildItemTooltipInfo(IOwnedUsable item)
    {
        BaseItem? definition = GetDefinition(item);
        string stack = item is IStackable stackable
            ? stackable.CurrentStackCount.ToString()
            : "1";

        return $"Weight: {definition?.Weight:0.##}\n" +
               $"Price: {definition?.Price}\n" +
               $"Stack: {stack}";
    }

    private static void UpdateItemTooltipInfo(TextBlock label, IOwnedUsable item)
    {
        string current = BuildItemTooltipInfo(item);
        if (!string.Equals(label.Text, current, StringComparison.Ordinal))
            label.Text = current;
    }

    private static BaseItem? GetDefinition(IOwnedUsable item)
        => (item as BaseItemINS)?.Definition;

    #endregion

    #region Actions and combat

    private void PrimaryAttack()
    {
        AttackComplex? attack = _character.PerformPrimaryAttack(_currentFoe.Actor);
        if (attack is null)
        {
            Console.WriteLine($"{_character.Name} cannot perform primary attack: no usable weapon.");
            return;
        }

        attack.ApplyAttackToTarget(_currentFoe.Actor);
        Console.WriteLine($"{_character.Name} hits {_currentFoe.Name} with primary attack for {attack.Damage} damage.");
        RefreshAll(0.0f);
    }

    private void SecondaryAttack()
    {
        AttackComplex? attack = _character.PerformSecondaryAttack(_currentFoe.Actor);
        if (attack is null)
        {
            Console.WriteLine($"{_character.Name} has no secondary attack available.");
            return;
        }

        attack.ApplyAttackToTarget(_currentFoe.Actor);
        Console.WriteLine($"{_character.Name} hits {_currentFoe.Name} with secondary attack for {attack.Damage} damage.");
        RefreshAll(0.0f);
    }

    private void RestoreCurrentFoe()
    {
        _currentFoe.Actor.RestoreVital("Health", 100000);
        Console.WriteLine($"{_currentFoe.Name} restored.");
        RefreshAll(0.0f);
    }

    private void PickUpSelected()
    {
        if (_pickupCombo.SelectedItem is not PickupEntry entry)
            return;

        IOwnedUsable item = entry.Create();
        _character.PickUpItem(item);
        Console.WriteLine($"{_character.Name} picked up {item.GetDefinitionName()}.");
        RefreshAll(0.0f);
    }

    #endregion

    #region Scenario loop

    private void ToggleScenario()
    {
        if (_scenarioCancellation is not null)
        {
            _scenarioCancellation.Cancel();
            return;
        }

        _scenarioCancellation = new CancellationTokenSource();
        _scenarioTask = RunScenarioLoopAsync(_scenarioCancellation.Token);
        Console.WriteLine("Current scenario started.");
    }

    private async Task RunScenarioLoopAsync(CancellationToken token)
    {
        var stopwatch = Stopwatch.StartNew();
        double previous = stopwatch.Elapsed.TotalSeconds;

        try
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                double now = stopwatch.Elapsed.TotalSeconds;
                float deltaTime = (float)Math.Clamp(now - previous, 0.0, 0.25);
                previous = now;

                _character.Update(deltaTime);
                _currentFoe.UpdateAction?.Invoke(deltaTime);
                CurrentScenarioUpdate(deltaTime);

                await Task.Delay(16, token);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Current scenario stopped.");
        }
        finally
        {
            _scenarioCancellation?.Dispose();
            _scenarioCancellation = null;
            _scenarioTask = null;
            RefreshAll(0.0f);
        }
    }

    #endregion

    #region Helpers

    private static Image CreateImage(string? path, Stretch stretch)
        => new()
        {
            Source = AssetLocator.LoadBitmap(path),
            Stretch = stretch
        };

    private static void Place(Canvas canvas, Control control,
        double x, double y, double width, double height)
    {
        control.Width = width;
        control.Height = height;
        Canvas.SetLeft(control, x);
        Canvas.SetTop(control, y);
        canvas.Children.Add(control);
    }

    private void OnLogLineAdded(string line)
        => Dispatcher.UIThread.Post(() => AppendLog(line));

    private void AppendLog(string line)
    {
        _consoleBox.Text = string.IsNullOrEmpty(_consoleBox.Text)
            ? line
            : _consoleBox.Text + Environment.NewLine + line;
        _consoleBox.CaretIndex = _consoleBox.Text?.Length ?? 0;
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        LogHub.LineAdded -= OnLogLineAdded;
        _scenarioCancellation?.Cancel();
        _scenarioCancellation?.Dispose();
    }

    #endregion
}
