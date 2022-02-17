import React, { Component } from 'react';
import { Redirect } from 'react-router-dom';
import authService from './api-authorization/AuthorizeService'

export class AddTests extends Component {
    constructor(props) {
        super(props);
        this.state = {created:false, success:true,errors:[]};
        this.handleSubmit = this.handleSubmit.bind(this);
    }
    render() {
        return (
            !this.state.created
                ? (<>
                    <h1>Создать тест</h1>
                    <form name="createTest" method="post">

                        <label for="TestName">Название теста</label>
                        <input type="text" name="TestName" id="TestName" />

                        <label for="IsPrivate">Доступен только по ссылке</label>
                        <input type="checkbox" name="IsPrivate" id="IsPrivate" checked />

                        <input type="submit" value="Создать тест" onClick={this.handleSubmit} />
                    </form>
                </>)
                : this.state.success
                    ? <Redirect to="/manage-tests" />
                    : AddTests.renderErrors(this.state.errors)
            );
    }
    handleSubmit() {
        const form = document.forms["createTest"];
        const formData = new FormData();
        formData.append("TestName", form.elements["TestName"].value);
        formData.append("IsPrivate", form.elemets["IsPrivate"].value);

        const accessToken = await authService.getAccessToken();
        const response = await fetch("api/createTest", {
            method: "POST",
            body: formData,
            headers: !!accessToken ? { "Authorization": `Bearer ${accessToken}` } : {}
        });
        const result = response.json();
        this.setState({ created: true, success: response.ok, errors: result.errors })
    }

    static renderErrors(errors) {
        return (<>
            {errors.map(error => {
                <div key={error.message}>
                    <h3>{error.message}</h3>
                </div>
            })}
        </>);
    }
}