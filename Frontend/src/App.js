import { useState } from "react";

import FileUploader from "./FIleUploader/FileUploader";

import "./App.css";

function App() {
  const [serverResponse, setServerResponse] = useState("");

  const displayServerError = (message) => {
    let id;
    const showServerMessage = (msg) => {
      clearTimeout(id);
      setServerResponse(msg);
      id = setTimeout(() => {
        setServerResponse("");
      }, 4000);
    };
    showServerMessage(message);
  };
  return (
    <div className="App">
      <FileUploader displayServerError={displayServerError} />
      {serverResponse === "" || (
        <p className="server-response">{serverResponse}</p>
      )}
    </div>
  );
}

export default App;
