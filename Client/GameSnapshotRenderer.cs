using PROJ.Builder.Classes;
using PROJ.Communication.Results;
using PROJ.Communication.Snapshots;
using PROJ.GameConstansts;

namespace PROJ.Client;

public static class GameSnapshotRenderer
{
    private static bool layoutRendered;
    private static string[]? previousBoard;
    private static RenderBuffer? previousActionBox;
    private static RenderBuffer? previousFightBox;
    private static RenderBuffer? previousVitalsBox;
    private static RenderBuffer? previousWealthBox;
    private static RenderBuffer? previousEquipmentBox;
    private static RenderBuffer? previousLeftHandBox;
    private static RenderBuffer? previousRightHandBox;
    private static RenderBuffer? previousLoggerBox;
    private static RenderBuffer? previousLoggerMarker;
    private static RenderBuffer? previousActionError;
    private static RenderBuffer? previousFightError;
    private static RenderBuffer? previousEndMessage;

    public static void Render(GameSnapshotMessage snapshot)
    {
        ConsoleRender.Run(() =>
        {
            Console.CursorVisible = false;

            if (!layoutRendered)
            {
                Console.Clear();
                RenderBoard(snapshot.Board);
                RenderStaticLayout();
                layoutRendered = true;
            }
            else
            {
                RenderBoard(snapshot.Board);
            }

            RenderBufferDiff(
                GameConstants.ActionBoxLeft + 1,
                GameConstants.ActionBoxTop + 1,
                CreateActionBoxBuffer(snapshot.ActionBox),
                ref previousActionBox);

            RenderBufferDiff(
                GameConstants.FightBoxLeft + 1,
                GameConstants.FightBoxTop + 1,
                CreateFightBoxBuffer(snapshot),
                ref previousFightBox);

            RenderBufferDiff(
                GameConstants.VitalsBoxLeft + 1,
                GameConstants.VitalsBoxTop + 1,
                CreateVitalsBuffer(snapshot.Player),
                ref previousVitalsBox);

            RenderBufferDiff(
                GameConstants.WealthBoxLeft + 1,
                GameConstants.WealthBoxTop + 1,
                CreateWealthBuffer(snapshot.Player),
                ref previousWealthBox);

            RenderBufferDiff(
                GameConstants.EqBoxLeft + 1,
                GameConstants.EqBoxTop + 1,
                CreateEquipmentBuffer(snapshot.Player),
                ref previousEquipmentBox);

            RenderBufferDiff(
                GameConstants.LeftHandBoxLeft + 1,
                GameConstants.LeftHandBoxTop + 1,
                CreateHandBuffer(snapshot.Player.LeftHand, true),
                ref previousLeftHandBox);

            RenderBufferDiff(
                GameConstants.RightHandBoxLeft + 1,
                GameConstants.RightHandBoxTop + 1,
                CreateHandBuffer(snapshot.Player.RightHand, false),
                ref previousRightHandBox);

            RenderBufferDiff(
                GameConstants.LoggerBoxLeft + 1,
                GameConstants.LoggerBoxTop + 1,
                CreateLoggerBuffer(snapshot.Logger),
                ref previousLoggerBox);

            RenderBufferDiff(
                GameConstants.LoggerBoxRight - "LOGGER MODE".Length - 2,
                GameConstants.LoggerBoxTop,
                CreateLoggerMarkerBuffer(snapshot.Logger),
                ref previousLoggerMarker);

            RenderBufferDiff(
                GameConstants.ActionBoxLeft,
                GameConstants.ErrorSpaceTop,
                CreateErrorBuffer(snapshot.Errors.ActionError, GameConstants.ActionBoxRight - GameConstants.ActionBoxLeft),
                ref previousActionError);

            RenderBufferDiff(
                GameConstants.FightBoxLeft,
                GameConstants.ErrorSpaceTop,
                CreateErrorBuffer(snapshot.Errors.FightError, GameConstants.FightBoxRight - GameConstants.FightBoxLeft),
                ref previousFightError);

            RenderBufferDiff(0, 0, CreateEndMessageBuffer(snapshot), ref previousEndMessage);
        }, preserveCursor: false);

        RenderEffects(snapshot.Effects);
    }

    private static void RenderStaticLayout()
    {
        Player dummyPlayer = new Player();

        new ActionBox().DisplayFrame();
        new FightBox().DisplayFrame();
        new VitalsBox(dummyPlayer).DisplayFrame();
        new WealthBox(dummyPlayer).DisplayFrame();
        new EquipmentBox(dummyPlayer).DisplayFrame();
        new LeftHandBox(dummyPlayer).DisplayFrame();
        new RightHandBox(dummyPlayer).DisplayFrame();

        PlayerMovesBox playerMovesBox = new PlayerMovesBox();
        PlayerMovesBuilder playerMovesBuilder = new PlayerMovesBuilder(playerMovesBox);
        playerMovesBuilder.AddInitial();
        playerMovesBuilder.AddPickup();
        playerMovesBuilder.AddEnemy();

        new LoggerBox().DisplayFrame();
    }

    private static void RenderBoard(string[] rows)
    {
        if (previousBoard == null)
            RenderBoardSigns();

        int height = Math.Min(GameConstants.Height, Math.Max(previousBoard?.Length ?? 0, rows.Length));
        for (int row = 0; row < height; row++)
        {
            string previousRow = previousBoard != null && row < previousBoard.Length ? previousBoard[row] : string.Empty;
            string currentRow = row < rows.Length ? rows[row] : string.Empty;
            int width = Math.Min(GameConstants.Width, Math.Max(previousRow.Length, currentRow.Length));

            for (int column = 0; column < width; column++)
            {
                char previous = column < previousRow.Length ? previousRow[column] : ' ';
                char current = column < currentRow.Length ? currentRow[column] : ' ';
                if (previous == current)
                    continue;

                Console.SetCursorPosition(GameConstants.BoardLeft + column, GameConstants.BoardTop + row);
                Console.Write(current);
            }
        }

        previousBoard = rows.ToArray();
    }

    private static void RenderBoardSigns()
    {
        string[] signLines = GameConstants.AboveBoardSign.Split('\n');
        for (int i = 0; i < signLines.Length; i++)
        {
            Console.SetCursorPosition(GameConstants.SignStartLeft, GameConstants.SignStartTop + i);
            Console.Write(signLines[i]);
        }

        string[] sign2Lines = GameConstants.BelowBoardSign.Split('\n');
        for (int i = 0; i < sign2Lines.Length; i++)
        {
            Console.SetCursorPosition(GameConstants.Sign2StartLeft, GameConstants.Sign2StartTop + i);
            Console.Write(sign2Lines[i]);
        }
    }

    private static RenderBuffer CreateActionBoxBuffer(ActionBoxSnapshot? snapshot)
    {
        RenderBuffer buffer = CreateBoxBuffer(
            GameConstants.ActionBoxLeft,
            GameConstants.ActionBoxRight,
            GameConstants.ActionBoxTop,
            GameConstants.ActionBoxBottom);

        if (snapshot == null || snapshot.Objects.Count == 0)
            return buffer;

        int selected = Math.Clamp(snapshot.Seek, 0, snapshot.Objects.Count - 1);
        BoardObjectSnapshot obj = snapshot.Objects[selected];
        int nameRow = GameConstants.ActionBoxWritingPointName - GameConstants.ActionBoxTop - 1;
        int descRow = GameConstants.ActionBoxWritingPointDesc - GameConstants.ActionBoxTop - 1;
        int pickupRow = GameConstants.ActionBoxWritingPointPickup - GameConstants.ActionBoxTop - 1;

        buffer.WriteCentered(nameRow, "You're standing on: " + obj.Name);
        buffer.WriteCentered(descRow, obj.Description);

        if (obj.Pickupable)
            buffer.WriteCentered(pickupRow, "Press 'E' to pick it up");

        if (selected > 0)
            buffer.WriteText(5, nameRow, "<---");

        if (selected < snapshot.Objects.Count - 1)
            buffer.WriteText(buffer.Width - 11, nameRow, "--->");

        return buffer;
    }

    private static RenderBuffer CreateFightBoxBuffer(GameSnapshotMessage snapshot)
    {
        RenderBuffer buffer = CreateBoxBuffer(
            GameConstants.FightBoxLeft,
            GameConstants.FightBoxRight,
            GameConstants.FightBoxTop,
            GameConstants.FightBoxBottom);

        if (snapshot.Fight == null)
        {
            if (snapshot.NearbyEnemy != null)
                WriteNearbyEnemy(buffer, snapshot.NearbyEnemy);
            return buffer;
        }

        FightSnapshot fight = snapshot.Fight;
        int iconRow = GameConstants.FightBoxPlayerIconTop - GameConstants.FightBoxTop - 1;
        int playerIconColumn = GameConstants.FightBoxPlayerIconLeft - GameConstants.FightBoxLeft - 1;
        int leftHandColumn = GameConstants.FightBoxPlayerLeftHand - GameConstants.FightBoxLeft - 1;
        int rightHandColumn = GameConstants.FightBoxPlayerRightHand - GameConstants.FightBoxLeft - 1;
        int attacksColumn = GameConstants.FightBoxAttacksLeft - GameConstants.FightBoxLeft - 1;
        int attacksTop = GameConstants.FightBoxAttacksTop - GameConstants.FightBoxTop - 1;
        int enemyColumn = GameConstants.FightBoxEnemyPositionLeft - GameConstants.FightBoxLeft - 1;
        int enemyRow = GameConstants.FightBoxEnemyPositionTop - GameConstants.FightBoxTop - 1;
        int enemyVitalsColumn = GameConstants.FightBoxEnemyVitalsLeft - GameConstants.FightBoxLeft - 1;
        int enemyVitalsTop = GameConstants.FightBoxEnemyVitalsTop - GameConstants.FightBoxTop - 1;

        buffer.WriteText(leftHandColumn, iconRow, fight.Player.LeftHandVisual.ToString(),
            fight.SelectedHand == 'L' ? ConsoleColor.Green : null);
        buffer.WriteText(playerIconColumn, iconRow, GameConstants.PlayerSymbol.ToString());
        buffer.WriteText(rightHandColumn, iconRow, fight.Player.RightHandVisual.ToString(),
            fight.SelectedHand == 'R' ? ConsoleColor.Green : null);

        buffer.WriteText(attacksColumn, attacksTop + 2, "1. Normal Attack",
            fight.SelectedAttack == 1 ? ConsoleColor.Green : null);
        buffer.WriteText(attacksColumn, attacksTop + 3, "2. Sneaky Attack",
            fight.SelectedAttack == 2 ? ConsoleColor.Green : null);
        buffer.WriteText(attacksColumn, attacksTop + 4, "3. Magic Attack",
            fight.SelectedAttack == 3 ? ConsoleColor.Green : null);

        buffer.WriteText(enemyColumn, enemyRow, fight.Enemy.Visual.ToString());
        buffer.WriteText(enemyVitalsColumn, enemyVitalsTop, "Health: " + fight.Enemy.Health);
        buffer.WriteText(enemyVitalsColumn, enemyVitalsTop + 1, "Armor: " + fight.Enemy.Armor);

        return buffer;
    }

    private static void WriteNearbyEnemy(RenderBuffer buffer, EnemyViewResult enemy)
    {
        int nameRow = GameConstants.FightBoxWritingPointName - GameConstants.FightBoxTop - 1;
        int descRow = GameConstants.FightBoxWritingPointDesc - GameConstants.FightBoxTop - 1;
        int pickupRow = GameConstants.FightBoxWritingPointPickup - GameConstants.FightBoxTop - 1;

        buffer.WriteCentered(nameRow, "You're Near: " + enemy.Name);
        buffer.WriteCentered(descRow, enemy.Description);

        if (enemy.Fightable)
            buffer.WriteCentered(pickupRow, "Press 'Enter' to fight");
    }

    private static RenderBuffer CreateVitalsBuffer(PlayerSnapshot player)
    {
        RenderBuffer buffer = CreateBoxBuffer(
            GameConstants.VitalsBoxLeft,
            GameConstants.VitalsBoxRight,
            GameConstants.VitalsBoxTop,
            GameConstants.VitalsBoxBottom);

        int left = GameConstants.VitalsBoxWritingPointStartLeft - GameConstants.VitalsBoxLeft - 1;
        int top = GameConstants.VitalsBoxWritingPointStartTop - GameConstants.VitalsBoxTop - 1;
        string[] lines =
        {
            $"Health: {player.Health}/100",
            $"Level: {Math.Floor(player.Level)}",
            $"Strength: {player.Strength}",
            $"Dexterity: {player.Dexterity}",
            $"Luck: {player.Luck}",
            $"Wisdom: {player.Wisdom}"
        };

        buffer.WriteLines(left, top, lines);
        return buffer;
    }

    private static RenderBuffer CreateWealthBuffer(PlayerSnapshot player)
    {
        RenderBuffer buffer = CreateBoxBuffer(
            GameConstants.WealthBoxLeft,
            GameConstants.WealthBoxRight,
            GameConstants.WealthBoxTop,
            GameConstants.WealthBoxBottom);

        int left = GameConstants.WealthBoxWritingPointStartLeft - GameConstants.WealthBoxLeft - 1;
        int top = GameConstants.WealthBoxWritingPointStartTop - GameConstants.WealthBoxTop - 1;
        buffer.WriteLines(left, top, new[]
        {
            $"Gold: {player.Gold}",
            $"Coins: {player.Coins}"
        });
        return buffer;
    }

    private static RenderBuffer CreateEquipmentBuffer(PlayerSnapshot player)
    {
        RenderBuffer buffer = CreateBoxBuffer(
            GameConstants.EqBoxLeft,
            GameConstants.EqBoxRight,
            GameConstants.EqBoxTop,
            GameConstants.EqBoxBottom);

        int left = GameConstants.EqBoxWritingPointStartLeft - GameConstants.EqBoxLeft - 1;
        int pointerColumn = GameConstants.EqPointer - GameConstants.EqBoxLeft - 1;
        int top = GameConstants.EqBoxWritingPointStartTop - GameConstants.EqBoxTop - 1;

        for (int i = 0; i < GameConstants.BackpackCapacity; i++)
        {
            ItemSnapshot? item = i < player.Backpack.Count ? player.Backpack[i] : null;
            string text = item != null
                ? $"{i + 1}. {item.Name} ({item.Space})"
                : $"{i + 1}. --------------";

            buffer.WriteText(left, top + i, text);

            if (player.IsInBackpack &&
                player.BackpackIndex >= 0 &&
                player.BackpackIndex < GameConstants.BackpackCapacity &&
                player.BackpackIndex == i)
            {
                buffer.WriteText(pointerColumn, top + i, "<");
            }
        }

        return buffer;
    }

    private static RenderBuffer CreateHandBuffer(ItemSnapshot? item, bool leftHand)
    {
        int boxLeft = leftHand ? GameConstants.LeftHandBoxLeft : GameConstants.RightHandBoxLeft;
        int boxRight = leftHand ? GameConstants.LeftHandBoxRight : GameConstants.RightHandBoxRight;
        int boxTop = leftHand ? GameConstants.LeftHandBoxTop : GameConstants.RightHandBoxTop;
        int boxBottom = leftHand ? GameConstants.LeftHandBoxBottom : GameConstants.RightHandBoxBottom;
        int startLeft = leftHand
            ? GameConstants.LeftHandBoxWritingPointStartLeftName
            : GameConstants.RightHandBoxWritingPointStartLeftName;
        int nameTop = leftHand
            ? GameConstants.LeftHandoxWritingPointStartTopName
            : GameConstants.RightHandoxWritingPointStartTopName;
        int descTop = leftHand
            ? GameConstants.LeftHandoxWritingPointStartTopDesc
            : GameConstants.RightHandoxWritingPointStartTopDesc;

        RenderBuffer buffer = CreateBoxBuffer(boxLeft, boxRight, boxTop, boxBottom);
        int relLeft = startLeft - boxLeft - 1;
        int relNameTop = nameTop - boxTop - 1;
        int relDescTop = descTop - boxTop - 1;

        buffer.WriteText(relLeft, relNameTop, item?.Name ?? "-------------");
        buffer.WriteText(relLeft, relDescTop, item != null ? "Icon " + item.Visual : "--------------------------");
        buffer.WriteText(relLeft, relDescTop + 3, item != null ? "Space " + item.Space : "-----");

        return buffer;
    }

    private static RenderBuffer CreateLoggerBuffer(LoggerSnapshot snapshot)
    {
        RenderBuffer buffer = CreateBoxBuffer(
            GameConstants.LoggerBoxLeft,
            GameConstants.LoggerBoxRight,
            GameConstants.LoggerBoxTop,
            GameConstants.LoggerBoxBottom);

        int maxTextLength = GameConstants.LoggerBoxRight - GameConstants.LoggerBoxWritingPointStartLeft;
        int maxLines = GameConstants.LoggerBoxBottom - GameConstants.LoggerBoxWritingPointStartTop;
        int visibleLines = Math.Min(snapshot.VisibleLines.Count, maxLines);

        for (int i = 0; i < visibleLines; i++)
        {
            string line = snapshot.VisibleLines[i];
            if (line.Length > maxTextLength)
                line = line[..maxTextLength];

            buffer.WriteText(0, i, line);
        }

        return buffer;
    }

    private static RenderBuffer CreateLoggerMarkerBuffer(LoggerSnapshot snapshot)
    {
        const string marker = "LOGGER MODE";
        RenderBuffer buffer = new RenderBuffer(marker.Length, 1);
        buffer.WriteText(0, 0, snapshot.IsInLoggerMode ? marker : new string('─', marker.Length));
        return buffer;
    }

    private static RenderBuffer CreateErrorBuffer(string? message, int width)
    {
        RenderBuffer buffer = new RenderBuffer(width, 1);

        if (string.IsNullOrWhiteSpace(message))
            return buffer;

        if (message.Length > width)
            message = message[..width];

        buffer.WriteCentered(0, message, ConsoleColor.Red);
        return buffer;
    }

    private static RenderBuffer CreateEndMessageBuffer(GameSnapshotMessage snapshot)
    {
        RenderBuffer buffer = new RenderBuffer(20, 1);
        if (snapshot.GameEnded)
            buffer.WriteText(0, 0, snapshot.EndedGood ? "Game Ended!" : "You died!");
        return buffer;
    }

    private static RenderBuffer CreateBoxBuffer(int left, int right, int top, int bottom)
    {
        return new RenderBuffer(right - left - 1, bottom - top - 1);
    }

    private static void RenderBufferDiff(int left, int top, RenderBuffer current, ref RenderBuffer? previous)
    {
        ConsoleColor defaultForeground = Console.ForegroundColor;
        RenderBuffer? old = previous;

        for (int y = 0; y < current.Height; y++)
        {
            int x = 0;
            while (x < current.Width)
            {
                if (!CellChanged(old, current, x, y))
                {
                    x++;
                    continue;
                }

                ConsoleColor? color = current.GetForeground(x, y);
                int start = x;
                while (x < current.Width &&
                       CellChanged(old, current, x, y) &&
                       current.GetForeground(x, y) == color)
                {
                    x++;
                }

                Console.SetCursorPosition(left + start, top + y);
                Console.ForegroundColor = color ?? defaultForeground;
                Console.Write(current.GetText(y, start, x - start));
            }
        }

        Console.ForegroundColor = defaultForeground;
        previous = current;
    }

    private static bool CellChanged(RenderBuffer? previous, RenderBuffer current, int x, int y)
    {
        char previousChar = previous != null && previous.Contains(x, y) ? previous.GetChar(x, y) : ' ';
        ConsoleColor? previousColor = previous != null && previous.Contains(x, y) ? previous.GetForeground(x, y) : null;

        return previousChar != current.GetChar(x, y) ||
               previousColor != current.GetForeground(x, y);
    }

    private static void RenderEffects(VisualEffectsSnapshot? snapshot)
    {
        if (snapshot == null)
            return;

        foreach (TileEffectSnapshot effect in snapshot.TileEffects)
        {
            if (effect.DelayMs <= 0)
                RenderEffect(effect);
            else
                _ = Task.Run(async () =>
                {
                    await Task.Delay(effect.DelayMs);
                    RenderEffect(effect);
                });
        }
    }

    private static void RenderEffect(TileEffectSnapshot effect)
    {
        TileBlinkResult blink = ToTileBlinkResult(effect);
        switch (effect.Kind)
        {
            case "sound":
                BoardView.SoundBlink(blink);
                break;
            case "blink":
                BoardView.SpecificBlink(blink);
                break;
        }
    }

    private static TileBlinkResult ToTileBlinkResult(TileEffectSnapshot effect)
    {
        TileSnapshot tile = effect.Tile;
        return new TileBlinkResult(
            new TileViewResult(tile.Row, tile.Column, tile.Visual, tile.IsEmpty),
            effect.Color);
    }

    private sealed class RenderBuffer
    {
        private readonly char[,] chars;
        private readonly ConsoleColor?[,] foreground;

        public RenderBuffer(int width, int height)
        {
            Width = width;
            Height = height;
            chars = new char[height, width];
            foreground = new ConsoleColor?[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    chars[y, x] = ' ';
            }
        }

        public int Width { get; }
        public int Height { get; }

        public bool Contains(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public char GetChar(int x, int y)
        {
            return chars[y, x];
        }

        public ConsoleColor? GetForeground(int x, int y)
        {
            return foreground[y, x];
        }

        public string GetText(int y, int start, int length)
        {
            char[] text = new char[length];
            for (int i = 0; i < length; i++)
                text[i] = chars[y, start + i];
            return new string(text);
        }

        public void WriteLines(int x, int y, IReadOnlyList<string> lines, ConsoleColor? color = null)
        {
            for (int i = 0; i < lines.Count; i++)
                WriteText(x, y + i, lines[i], color);
        }

        public void WriteCentered(int y, string text, ConsoleColor? color = null)
        {
            if (y < 0 || y >= Height)
                return;

            if (text.Length > Width)
                text = text[..Width];

            int x = Math.Max(0, (Width - text.Length) / 2);
            WriteText(x, y, text, color);
        }

        public void WriteText(int x, int y, string text, ConsoleColor? color = null)
        {
            if (y < 0 || y >= Height)
                return;

            for (int i = 0; i < text.Length; i++)
            {
                int column = x + i;
                if (column < 0)
                    continue;
                if (column >= Width)
                    break;

                chars[y, column] = text[i];
                foreground[y, column] = color;
            }
        }
    }
}
