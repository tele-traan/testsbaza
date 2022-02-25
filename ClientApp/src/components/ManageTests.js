import React, { Component } from 'react'
import { TestSummary } from './TestSummary'
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
                    <TestSummary id={test.Id} name={test.TestName} date={test.TimeCreated} />
                    <a href={`/edit-test?testId=${test.Id}`}>Редактировать тест</a>
                </div>
            })}

        </>)
    }
}