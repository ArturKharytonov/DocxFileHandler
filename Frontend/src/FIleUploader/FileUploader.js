import React, { useState } from "react";
import axios from "axios";
import "./FileUploader.css";

function FileUploader({displayServerError}) {
  const [selectedFile, setSelectedFile] = useState(null);
  const [email, setEmail] = useState("");
  const [emailError, setEmailError] = useState("");

  const handleFileChange = (event) => {
    const file = event.target.files[0];
    setSelectedFile(file);
  };

  const handleDrop = (event) => {
    event.preventDefault();
    const file = event.dataTransfer.files[0];
    setSelectedFile(file);
  };

  const handleDragOver = (event) => {
    event.preventDefault();
  };

  const handleDragEnter = (event) => {
    event.preventDefault();
    event.target.classList.add("active");
  };

  const handleDragLeave = (event) => {
    event.preventDefault();
    event.target.classList.remove("active");
  };

  const validateEmail = (newEmail) => {
    const allowedCharacters = /^[\w-]+(\.[\w-]+)*@[\w-]+(\.[a-zA-Z]{2,})+$/;
    return allowedCharacters.test(newEmail);
  };

  const handleEmailChange = (event) => {
    const newEmail = event.target.value;
    setEmail(newEmail);
    setEmailError(validateEmail(newEmail) ? "" : "Invalid email address");
  };

  const handleUpload = async () => {
    if (!selectedFile || !validateEmail(email)) {
      console.error("File or email is missing or invalid.");
      return;
    }

    const formData = new FormData();
    formData.append("file", selectedFile);
    displayServerError("Sending...")
    try {
      const response = await axios.post(
        `https://docxfilehandlerapi20230917114027.azurewebsites.net/api/FileUpload?email=${email}`,
        formData
      );
      console.log("File uploaded successfully:", response.data);
      displayServerError(response.data)
    } catch (error) {
      console.error("Error uploading file:", error);
      displayServerError(error.message)
    }
  };

  return (
    <div className="main-div">
      <div>
        <h2 className="main-div-h2">File Uploader</h2>
        <div className="email-zone">
          <input
            type="text"
            className="email-input"
            placeholder="email"
            value={email}
            onChange={handleEmailChange}
          />
          {emailError && email && <p className="email-error">{emailError}</p>}
        </div>
        <div
          className={`drop-zone`}
          onDrop={handleDrop}
          onDragOver={handleDragOver}
          onDragEnter={handleDragEnter}
          onDragLeave={handleDragLeave}
        >
          <p>Drag and drop a file here, or</p>
          <input
            type="file"
            accept=".docx"
            onChange={handleFileChange}
            style={{ display: "none" }}
          />
          <button
            onClick={() => document.querySelector('input[type="file"]').click()}
          >
            Select a file
          </button>
        </div>
        {selectedFile && (
          <div className="selected-file-zone">
            <p>Selected file: {selectedFile.name}</p>
            <button
              disabled={!selectedFile || emailError || !email}
              id="animateButton"
              onClick={(event) => {
                event.preventDefault(); 
            
                document.querySelector('.main-div').classList.add('spin-animation');
            
                setTimeout(() => {
                  document.querySelector('.main-div').classList.remove('spin-animation');
                }, 2000);
            
                handleUpload();
              }}
            >
              Upload
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

export default FileUploader;
