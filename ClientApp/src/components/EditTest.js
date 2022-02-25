import React, { Component } from 'react'
import { useLocation, Redirect } from 'react-router-dom';
import authService from './api-authorization/AuthorizeService'

export class EditTest extends Component {
    constructor(props) {
        super(props);
        this.state = { isLoading:true, success:false, test: {}};
        this.getTest = this.getTest.bind(this);
        this.renderTest = this.renderTest.bind(this);
    }
    componentDidMount() {
        this.getTest();
    }
    render() {
        return (
            this.state.isLoading
                ? <h1>Загрузка...</h1>
                : this.state.success ? this.renderTest()
                    : <Redirect to="/" />
                )
    }
    async getTest() {
        const testId = new URLSearchParams(window.location.search).get('testId');
        const accessToken = await authService.getAccessToken();
        const response = await fetch(`/api/test/get-test?testId=${testId}`, {
            headers: !accessToken ? {} : { 'Authorization': `Bearer ${accessToken}` }
        });
        if (response.ok) {
            response.json().then(result => {
                this.setState({ isLoading: false, success:true, test: result })
            }); 
        } else if (response.status === 404) {

        }
    }
    renderTest() {
        const test = this.state.test;
        return (<>
            <h1>Тест {test.TestName}</h1>
            <h2>Сложность: {test.Difficulty}</h2>
            <h2>Вопросы: </h2>
            {test.Questions.length === 0 ? <h4>В этот тест ещё не добавлены вопросы</h4> : 
                test.questions.map(q => {
                    <div key={q.Id}>
                    <h3><b>Вопрос {q.Number}</b></h3>
                    <h3>{q.Question}</h3>
                    <h3>Ответ: {q.Answer}</h3>
                    </div>
                })
            }
            </>);
    }
}