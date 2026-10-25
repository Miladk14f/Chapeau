package nl.inholland.quizgame.service;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import nl.inholland.quizgame.factory.QuestionFactory;
import nl.inholland.quizgame.model.Question;
import nl.inholland.quizgame.model.Quiz;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class QuizLoader {

    private static final ObjectMapper MAPPER = new ObjectMapper();

    private QuizLoader() {}

    public static Quiz load(File file) throws IOException {
        if (!file.exists()) {
            throw new IOException("File not found: " + file.getName());
        }
        if (file.length() == 0) {
            throw new IOException("File is empty: " + file.getName());
        }

        JsonNode root;
        try {
            root = MAPPER.readTree(file);
        } catch (Exception e) {
            throw new IOException("File is not valid JSON: " + file.getName());
        }

        if (!root.isObject() || !root.has("pages")) {
            throw new IOException("File is not a valid quiz (missing 'pages'): " + file.getName());
        }

        String quizId = root.path("quizId").asText("unknown");
        String title = root.path("title").asText("Untitled Quiz");
        String description = root.path("description").asText("");

        List<Question> questions = new ArrayList<>();
        JsonNode pages = root.path("pages");
        if (pages.isArray()) {
            for (JsonNode page : pages) {
                int timeLimit = page.path("timeLimit").asInt(30);
                JsonNode elements = page.path("elements");
                if (elements.isArray() && !elements.isEmpty()) {
                    // one question per page per assignment spec
                    questions.add(QuestionFactory.create(elements.get(0), timeLimit));
                }
            }
        }

        if (questions.isEmpty()) {
            throw new IOException("Quiz has no questions: " + file.getName());
        }

        return new Quiz(quizId, title, description, questions);
    }
}
