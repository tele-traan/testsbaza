import React, { Component } from 'react'
import authService from './api-authorization/AuthorizeService'

export default class TestSummary extends Component {
    constructor(props) {
        super(props);
        this.state = { isDeleted: false };
        this.handleDelete = this.handleDelete.bind(this);
    }
    render() {
        return (<>
            <h1>{this.props.name}</h1>
            <h3>{this.props.description}</h3>
            <h4>{this.props.difficulty}</h4>
            <a></a>
            <button onClick={this.handleDelete}>Удалить тест</button>
        </>)
    }
    async handleDelete() {
        if (!confirm(`Вы уверены, что хотите удалить тест ${this.props.name} навсегда?`)) return;
        const accessToken = await authService.getAccessToken();
        await fetch('/api/test/delete-test', {
            method: 'POST',
            body: JSON.stringify({ 'testId': this.props.testId }),
            headers: !accessToken ? {} : { 'Authorization': `Bearer ${accessToken}` }
        }).then(response => {
            if (response.ok) {

            } else {

            }
        });
    }
}