package nl.inholland.quizgame;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;

public class App extends Application {

    @Override
    public void start(Stage primaryStage) throws Exception {
        Parent root = FXMLLoader.load(
                getClass().getResource("/nl/inholland/quizgame/menu-view.fxml"));
        Scene scene = new Scene(root, 800, 600);
        scene.getStylesheets().add(
                getClass().getResource("/nl/inholland/quizgame/styles.css").toExternalForm());
        primaryStage.setTitle("Java Quiz Game");
        primaryStage.setScene(scene);
        primaryStage.setMinWidth(600);
        primaryStage.setMinHeight(450);
        primaryStage.show();
    }

    public static void main(String[] args) {
        launch(args);
    }
}
