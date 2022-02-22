import React, { Component } from 'react';
import { Redirect } from 'react-router-dom';
import authService from './api-authorization/AuthorizeService'

export class AddTest extends Component {
    constructor(props) {
        super(props);
        this.state = {created:false, success:true,errors:[]};
        this.handleSubmit = this.handleSubmit.bind(this);
    }
    render() {
        return (
            this.state.created ?
                  (this.state.success ?
                     <Redirect to="/manage-tests" />
                    : AddTest.renderErrors(this.state.errors))
            : (<>
            <h1>Создать тест</h1>
            <form name="createTest" method="post" encType="multipart/form-data">

                <label>Название теста</label><br />
                <input type="text" name="TestName" id="TestName" /><br />

                <label>Доступен только по ссылке</label><br />
                <input type="checkbox" name="IsPrivate" id="IsPrivate" defaultChecked value="true" /><br />

                <input type="submit" value="Создать тест" onClick={this.handleSubmit} />
            </form>
        </>)
            );
    }
    async handleSubmit(e) {
        e.preventDefault();
        const form = document.forms["createTest"];
        const formData = new FormData(form);
        /*formData.append("model.TestName", form.elements["TestName"].value);
        formData.append("model.IsPrivate", form.elements["IsPrivate"].value);
        alert(form.elements["IsPrivate"].checked)*/
        /*const formData = JSON.stringify({
            TestName: form.elements["TestName"].value,
            IsPrivate: form.elements["IsPrivate"].value
        });*/
        const accessToken = await authService.getAccessToken();
        const response = await fetch("/api/test/add-test", {
            method: "POST",
            body: formData,
            headers: !accessToken ? {} : {'Authorization': `Bearer ${accessToken}`}
        });
        if (response.ok) this.setState({ created: true, success: true });
        else {
            response.json().then(res => this.setState({ created: true, success: false, errors: res.errors }));
            alert(`is response.ok - ${response.ok}`);
        }
    }

    static renderErrors(errors) {
        return (<>
            <h1>{errors.TestName}</h1>
            <h1>{errors.IsPrivate}</h1>
        </>);
    }
}