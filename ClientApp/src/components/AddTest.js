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
        return (<>
            {this.state.created ?
                this.state.success ?
                    <Redirect to="/manage-tests" />
                    : <h1>Произошла ошибка при создании теста</h1>
                : <></>}

            <h1>Создать тест</h1>
            <form name="createTest" method="post" encType="multipart/form-data">

                <span>{this.state.errors["TestName"]}</span>
                <label>Название теста</label><br />
                <input type="text" name="TestName" id="TestName" /><br />

                <label>Доступен только по ссылке</label><br />
                <input type="checkbox" name="IsPrivate" id="IsPrivate" defaultChecked value="true" /><br />

                <input type="submit" value="Создать тест" onClick={this.handleSubmit} />
            </form>
        </>
            )
    }
    async handleSubmit(e) {
        e.preventDefault();
        const form = document.forms["createTest"];
        const formData = new FormData(form);
        const accessToken = await authService.getAccessToken();
        console.log(`access token: ${accessToken}`);
        const response = await fetch("/api/test/add-test", {
            method: "POST",
            body: formData,
            headers: !accessToken ? {} : {'Authorization': `Bearer ${accessToken}`}
        });
        let errors;
        if (response.ok) this.setState({ created: true, success: true, errors: ['bal'] });
        else {
         if (response.status === 401) errors = ["Ошибка при попытке авторизоваться. Попробуйте войти в аккаунт ещё раз"];
            else if (response.status === 403) errors = ["Тест с таким именем уже существует"];
            else errors = [`Ошибка ${response.status}. Попробуйте перезагрузить страницу и попробовать ещё раз`];
        }
        this.setState({ created: true, success: false, errors: errors})
    }
    static renderErrors(errors){
        return <>
            <h1>Произошли ошибки при создании теста</h1>
            <ul>
            {errors.map(error => {
                <li><h2>{error}</h2></li>
            })}
            </ul>
        </>;
    }
}