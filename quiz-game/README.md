# Java Quiz Game

A JavaFX desktop quiz application built for the Java Fundamentals practical assignment 2026–2027 at Hogeschool Inholland.

---

## 1. How to Run

**Requirements:** Java 21+, Maven 3.8+, IntelliJ IDEA (with JavaFX support)

**From the terminal inside the `quiz-game` directory:**

```bash
mvn javafx:run
```

**From IntelliJ:**
1. Open the `quiz-game` folder as a Maven project.
2. Let IntelliJ import the `pom.xml`.
3. Run `App.java` using the JavaFX Maven plugin (`javafx:run` in the Maven panel), or configure a Run Configuration with main class `nl.inholland.quizgame.App` and add the VM options below if needed:

```
--module-path <path-to-javafx-sdk>/lib --add-modules javafx.controls,javafx.fxml
```

A sample `quiz.json` is included in the project root. Open it with the **Load Quiz** button.

---

## 2. Design Rationale

### MVC (mandatory)
Every screen is an FXML file with its own controller. `menu-view.fxml`, `game-view.fxml`, and `results-view.fxml` each pair with `MenuController`, `GameController`, and `ResultsController`. The model classes (`Quiz`, `Question` and its subclasses, `QuizResult`) contain no JavaFX imports and know nothing about the user interface. Controllers read from and write to the model through `GameManager`.

### Singleton — `GameManager`
`GameManager` (in `nl.inholland.quizgame.manager`) is the single source of game state: the player name, the loaded quiz, the current question index, and the score. All three controllers obtain the same instance via `GameManager.getInstance()`. Without the Singleton, each controller would hold its own copy and state would diverge between screens.

### Observer — `scoreProperty` in `GameManager`
`scoreProperty` is a `SimpleIntegerProperty`. In `GameController.initialize()`, the score label binds to it with `Bindings.concat(...)`. When `gameManager.incrementScore()` is called, the property fires and the label updates on its own — no controller tells it to. This is the JavaFX built-in Observer implementation the assignment requires.

### Factory — `QuestionFactory`
`QuestionFactory.create(JsonNode, int)` reads the `"type"` field and returns the correct `Question` subclass. Adding a new question type means adding one class and one `case` line in the switch — no changes anywhere else in the loading or game logic.

### Template Method — `Question` hierarchy
`Question` declares `evaluate(String)` as a `final` method that calls the abstract `checkAnswer(String)`. This fixes the skeleton (null/blank guard → subclass check) while leaving the type-specific comparison to `RadioGroupQuestion` and `BooleanQuestion`. `UnknownQuestion` always returns `false` so the game keeps running even for unrecognised types. `getChoices()` is also abstract so the game screen can render each type's options without knowing the type.

### State — `MenuState`, `PlayingState`, `ResultsState`
`AppState` is an interface in `nl.inholland.quizgame.state`. `GameManager` holds the current state. `MenuController` checks `currentState.canStartQuiz(quizLoaded)` to decide whether to enable the Start button. `PlayingState.canLoadQuiz()` returns `false`, preventing load during a game. The state is changed on navigation: `PlayingState` when Start is pressed, `ResultsState` when the last question is answered, `MenuState` on Back to Menu.

### What I would change with more time
The `GameController` is slightly long because it handles both question display and timer logic. Given more time I would extract a `QuestionRenderer` that builds the choices VBox, and a `QuestionTimer` service that wraps the `Timeline`, making each class's purpose clearer.

---

## 3. Use of AI

Claude (claude.ai) was used to assist with:
- Generating the initial scaffolding of all source files (model classes, controllers, FXML layouts, CSS) based on the assignment specification.
- Suggesting the Jackson tree-model (`JsonNode`) approach for defensive JSON reading.
- Generating the CSS colour palette and progress-bar style classes.

All code was reviewed and understood before committing. The architecture decisions (which pattern goes where, how `scoreProperty` binds) are my own design choices, verified against the assignment requirements. The AI was used as a productivity tool, consistent with the course AI policy.

---

## 4. Known Limitations

- The `--add-opens` VM option may be needed on some JVM configurations; the `pom.xml` passes it via the JavaFX Maven plugin.
- The results file is saved in the directory where the application is launched from, not alongside the quiz file.
- Unknown question types are displayed with an explanatory message and skipped (not scored), which is correct per the robustness requirements.
- The UML diagram (`uml-diagram.png`) is a hand-drawn export; if the code diverges during late changes, the diagram may be slightly out of date in minor field names.
