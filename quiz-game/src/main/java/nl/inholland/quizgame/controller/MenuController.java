package nl.inholland.quizgame.controller;

import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.Alert;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.TextInputDialog;
import javafx.stage.FileChooser;
import javafx.stage.Modality;
import javafx.stage.Stage;
import nl.inholland.quizgame.manager.GameManager;
import nl.inholland.quizgame.model.Quiz;
import nl.inholland.quizgame.service.QuizLoader;
import nl.inholland.quizgame.state.PlayingState;

import java.io.File;
import java.io.IOException;
import java.util.Optional;

public class MenuController {

    @FXML private Button startQuizButton;
    @FXML private Label statusLabel;
    @FXML private Label quizTitleLabel;

    private final GameManager gameManager = GameManager.getInstance();

    @FXML
    public void initialize() {
        refreshView();
    }

    @FXML
    private void onLoadQuiz() {
        FileChooser chooser = new FileChooser();
        chooser.setTitle("Open Quiz File");
        chooser.getExtensionFilters().add(
                new FileChooser.ExtensionFilter("JSON files", "*.json"));

        Stage stage = (Stage) startQuizButton.getScene().getWindow();
        File file = chooser.showOpenDialog(stage);

        if (file == null) {
            return; // user cancelled — nothing to do
        }

        try {
            Quiz quiz = QuizLoader.load(file);
            gameManager.setCurrentQuiz(quiz);
            quizTitleLabel.setText("Loaded: " + quiz.getTitle());
            statusLabel.setText(quiz.getQuestionCount() + " questions ready.");
            refreshView();
        } catch (IOException e) {
            showError("Could not load quiz", e.getMessage());
        }
    }

    @FXML
    private void onStartQuiz() {
        if (gameManager.getCurrentQuiz() == null) {
            showError("No quiz loaded", "Please load a quiz file first.");
            return;
        }

        Optional<String> nameInput = askPlayerName();
        if (nameInput.isEmpty()) {
            return; // user cancelled
        }
        String name = nameInput.get().trim();
        if (name.isBlank()) {
            showError("Invalid name", "Please enter a name before starting.");
            return;
        }

        gameManager.startGame(name);
        gameManager.setState(new PlayingState());

        try {
            Parent gameRoot = FXMLLoader.load(
                    getClass().getResource("/nl/inholland/quizgame/game-view.fxml"));
            Stage stage = (Stage) startQuizButton.getScene().getWindow();
            stage.setScene(new Scene(gameRoot, 800, 600));
        } catch (IOException e) {
            showError("Navigation error", "Could not open game screen: " + e.getMessage());
        }
    }

    private Optional<String> askPlayerName() {
        TextInputDialog dialog = new TextInputDialog();
        dialog.setTitle("Enter Your Name");
        dialog.setHeaderText("What is your name?");
        dialog.setContentText("Name:");
        dialog.initModality(Modality.APPLICATION_MODAL);
        dialog.initOwner((Stage) startQuizButton.getScene().getWindow());
        return dialog.showAndWait();
    }

    private void refreshView() {
        boolean quizLoaded = gameManager.getCurrentQuiz() != null;
        startQuizButton.setDisable(!gameManager.getCurrentState().canStartQuiz(quizLoaded));
        if (!quizLoaded) {
            quizTitleLabel.setText("No quiz loaded");
            statusLabel.setText("Use 'Load Quiz' to open a quiz file.");
        }
    }

    private void showError(String title, String message) {
        Alert alert = new Alert(Alert.AlertType.ERROR);
        alert.setTitle(title);
        alert.setHeaderText(title);
        alert.setContentText(message);
        alert.initModality(Modality.APPLICATION_MODAL);
        alert.showAndWait();
    }
}
