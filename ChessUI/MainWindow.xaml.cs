﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChessLogic;

namespace ChessUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Image[,] pieceImages = new Image[8, 8];
    private GameState gameState;

    public MainWindow()
    {
        InitializeComponent();
        InitializeBoard();
        gameState = new GameState(Player.White, Chessboard.Initial());
        DrawBoard(gameState.Chessboard);
    }

    private void InitializeBoard()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Image image = new Image();
                pieceImages[row, col] = image;
                PieceGrid.Children.Add(image);
            }
        }
    }

    private void DrawBoard(Chessboard chessboard)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Piece piece = chessboard[row, col];
                pieceImages[row, col].Source = Images.GetImage(piece);

            }
        }
    }
}