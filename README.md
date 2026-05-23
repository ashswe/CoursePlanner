# Course Planner App

A mobile academic planning application built with .NET MAUI that allows users to manage academic terms, courses, assessments, notes, and progress tracking in a single organized interface.

---

## Features

- User authentication with hashed passwords
- Create, edit, and delete academic terms
- Add up to six courses per term
- Track course status and instructor information
- Create objective and performance assessments
- Course and assessment notifications
- Progress reporting and course completion tracking
- Local SQLite database storage
- Share course notes functionality
- Input validation and error handling

---

## Built With

- **.NET MAUI**
- **C#**
- **SQLite**
- **Visual Studio 2022**
- **Plugin.LocalNotification**

---

## Application Structure

The application follows a layered structure using models, services, and database access classes to improve maintainability and scalability.

### Main Components

| Component | Purpose |
|---|---|
| Models | Store application data |
| AppDatabase | SQLite CRUD operations |
| Services | Notification and sharing functionality |
| Pages | User interface and navigation |
| Validation Logic | Input and business rule validation |

---

## Database

The application uses a local SQLite database stored within the device application directory.

### Main Tables

- Users
- Terms
- Courses
- Assessments

---

## Security Features

- Salted SHA-256 password hashing
- Input validation
- Error handling and validation messages

---

---

## License

This project is for educational purposes.
