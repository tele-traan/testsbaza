import React, { Component } from 'react'
import authService from './api-authorization/AuthorizeService'

export class ManageTests extends Component {
    constructor(props) {
        super(props);
        this.state = { isLoading:true, isSuccess:false, tests:[] };
        this.populateTests = this.populateTests.bind(this);
        this.renderTests = this.renderTests.bind(this);
        this.handleDelete = this.handleDelete.bind(this);
    }
    componentDidMount() {
        this.populateTests();
    }
    render() {
        const content =
            this.state.isLoading
                ? <h1>Загрузка ваших тестов...</h1>
                : (this.state.isSuccess
                    ? this.renderTests()
                    : <h1>Ошибка</h1>)
        return (content);
    }
    async populateTests() {
        const accessToken = authService.getAccessToken();
        await fetch('/api/test/get-users-tests', {
            method: 'GET',
            headers: !accessToken ? {} : {'Authorization': `Bearer ${accessToken}`}
        }).then(response => {
            if (response.ok) {
                let result = response.json();
                this.setState({ isLoading: false, isSuccess: true, tests: result.tests });
            } else {
                this.setState({ isLoading: false, isSuccess: false, tests : [] });
            }
        });
    }
    renderTests() {
        return (<>

            {this.state.tests.map(test => {
                <div key={test.Id}>
                    <h1>Тест {test.name}</h1>
                    <h2>Дата создания: {test.dateCreated}</h2>
                    <a href={`/edit-test?testId=${test.Id}`}>Редактировать тест</a>
                    <form>
                        <input type="button" onClick={e => this.handleDelete(e)} value="Удалить тест" />
                        <input type="hidden" value={test.Id} name="id" />
                    </form>
                </div>
            })}

        </>)
    }
    async handleDelete(e) {
        const form = e.target.form;
        const testId = form.elements["id"].value;
        const accessToken = await authService.getAccessToken();
        await fetch('/api/test/delete-test', {
            method: 'POST',
            body: JSON.stringify({'testId': testId}),
            headers: !accessToken ? {} : { 'Authorization': `Bearer ${accessToken}` }
        }).then(response => {
            if (response.ok) {
                response.json().then();
            } else {
                alert('')
            }
        })
    }
}